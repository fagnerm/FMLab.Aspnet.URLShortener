// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.DTOs;
using FMLab.Aspnet.URLShortener.Business.Entities;
using FMLab.Aspnet.URLShortener.Business.Exceptions;
using FMLab.Aspnet.URLShortener.Business.Options;
using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Business.Services.Cache;
using FMLab.Aspnet.URLShortener.Business.Services.Identifier;
using FMLab.Aspnet.URLShortener.Business.Shared.Result;
using FMLab.Aspnet.URLShortener.Business.ValueObjects;
using Microsoft.Extensions.Options;

namespace FMLab.Aspnet.URLShortener.Business.Services.URL;
public partial class UrlService(IIdentifierService idService, IUrlRepository repository, IUrlClickRepository clickRepository, IUrlCacheService cache, IOptions<AppOptions> options) : IUrlService
{
    private readonly IUrlRepository _repository = repository;
    private readonly IUrlClickRepository _clickRepository = clickRepository;
    private readonly IUrlCacheService _cache = cache;
    private readonly IIdentifierService _idService = idService;
    private readonly IOptions<AppOptions> _options = options;

    public async Task<Result<CreateUrlOutputDTO>> RegisterUrlAsync(CreateUrlInputDTO input, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(input.Alias) && !AliasRegex().IsMatch(input.Alias))
            return Result<CreateUrlOutputDTO>.Validation("Alias must contain only letters, numbers and hyphens, up to 15 characters.");

        var target = new Url(input.Target);
        var id = await _idService.GetIdAsync();
        var url = new UrlRedirection(id, target, input.TemporaryRedirection, input.Alias);

        try
        {
            await _repository.AddAsync(url, cancellationToken);
        }
        catch (DomainException ex) when (ex.Message == "Url already exists")
        {
            return Result<CreateUrlOutputDTO>.Conflict(ex.Message);
        }

        var result = new CreateUrlOutputDTO($"{_options.Value.Domain}/{url.Hash}");
        return Result<CreateUrlOutputDTO>.Success(result);
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^[a-zA-Z0-9\-]{1,15}$")]
    private static partial System.Text.RegularExpressions.Regex AliasRegex();

    public async Task<Result<NoOutput>> DeleteUrlAsync(DeleteUrlInputDTO input, CancellationToken cancellationToken)
    {
        var existingUrl = await _repository.GetByHashAsync(input.Hash, cancellationToken);

        if (existingUrl is null) return Result<NoOutput>.NotFound("Url not found");

        await _repository.Delete(existingUrl!);
        await _cache.RemoveAsync(input.Hash);

        return Result<NoOutput>.NoContent();
    }

    public async Task<Result<UrlRedirectionOutputDTO>> LoadUrlRedirection(UrlRedirectionInputDTO input, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetAsync(input.Hash);
        if (cached is not null)
            return Result<UrlRedirectionOutputDTO>.Success(cached);

        var url = await _repository.GetByHashAsync(input.Hash, cancellationToken);

        if (url is null)
            return Result<UrlRedirectionOutputDTO>.NotFound("Url not found");

        var result = new UrlRedirectionOutputDTO(url.Target.Value, url.TemporaryRedirection);
        await _cache.SetAsync(input.Hash, result);

        return Result<UrlRedirectionOutputDTO>.Success(result);
    }

    public async Task RecordClickAsync(RecordClickInputDTO input, CancellationToken cancellationToken)
    {
        var click = new UrlClick(input.Hash, input.IpAddress, input.UserAgent, input.Referer);
        await _clickRepository.AddAsync(click, cancellationToken);
    }

    public async Task<Result<UrlAnalyticsOutputDTO>> GetAnalyticsAsync(UrlAnalyticsInputDTO input, CancellationToken cancellationToken)
    {
        var url = await _repository.GetByHashAsync(input.Hash, cancellationToken);

        if (url is null) return Result<UrlAnalyticsOutputDTO>.NotFound("Url not found");

        var totalClicks    = await _clickRepository.CountByHashAsync(input.Hash, cancellationToken);
        var clicksByDay    = await _clickRepository.GetDailyClicksAsync(input.Hash, AppOptions.ANALYTICS_PERIOD, cancellationToken);
        var topReferrers   = await _clickRepository.GetTopReferrersAsync(input.Hash, AppOptions.TOP_REFERRERS, cancellationToken);

        var result = new UrlAnalyticsOutputDTO(
            url.Hash,
            url.Target.Value,
            url.CreatedAt,
            totalClicks,
            clicksByDay,
            topReferrers
        );

        return Result<UrlAnalyticsOutputDTO>.Success(result);
    }

    public async Task<Result<UpdateUrlOutputDTO>> UpdateUrlAsync(UpdateUrlInputDTO input, CancellationToken cancellationToken)
    {
        var url = await _repository.GetByHashAsync(input.Hash, cancellationToken);

        if (url is null) return Result<UpdateUrlOutputDTO>.NotFound("Url not found");

        var target = string.IsNullOrEmpty(input.Target) ? url.Target : new Url(input.Target!);
        var redirection = input.TemporaryRedirection ? input.TemporaryRedirection : url.TemporaryRedirection;
        url.Update(target, redirection);

        await _repository.Update(url);
        await _cache.RemoveAsync(input.Hash);

        var result = new UpdateUrlOutputDTO(url.Hash, url.Target.Value, url.TemporaryRedirection);
        return Result<UpdateUrlOutputDTO>.Success(result);
    }
}
