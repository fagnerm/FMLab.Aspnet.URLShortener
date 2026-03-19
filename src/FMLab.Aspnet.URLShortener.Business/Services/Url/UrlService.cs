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

    public async Task<Result<CreateUrlOutputDTO>> CreateAsync(CreateUrlInputDTO input, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(input.Alias) && !AliasRegex().IsMatch(input.Alias))
            return Result<CreateUrlOutputDTO>.Failure("Alias must contain only letters, numbers and hyphens, up to 15 characters.");

        var target = new Url(input.Target);
        var id = await _idService.GetIdAsync();
        var url = new UrlRedirection(id, target, input.TemporaryRedirection, input.Alias);

        try
        {
            await _repository.AddAsync(url, cancellationToken);
        }
        catch (DomainException ex) when (ex.Message == "Url already exists")
        {
            return Result<CreateUrlOutputDTO>.Failure(ex.Message, ResultErrorType.Confict);
        }

        var result = new CreateUrlOutputDTO($"{_options.Value.Domain}/{url.Hash}");
        return Result<CreateUrlOutputDTO>.Success(result);
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^[a-zA-Z0-9\-]{1,15}$")]
    private static partial System.Text.RegularExpressions.Regex AliasRegex();

    public async Task<Result> DeleteAsync(DeleteUrlInputDTO input, CancellationToken cancellationToken)
    {
        var existingUrl = await _repository.GetByHashAsync(input.Hash, cancellationToken);

        if (existingUrl is null) return Result.Failure("Url not found", ResultErrorType.NotFound);

        await _repository.Delete(existingUrl!);
        await _cache.RemoveAsync(input.Hash);

        return Result.Success();
    }

    public async Task<Result<UrlRedirectionOutputDTO>> LoadUrlAsync(UrlRedirectionInputDTO input, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetAsync(input.Hash);
        if (cached is not null)
            return Result<UrlRedirectionOutputDTO>.Success(cached);

        var url = await _repository.GetByHashAsync(input.Hash, cancellationToken);

        if (url is null)
            return Result<UrlRedirectionOutputDTO>.Failure("Url not found", ResultErrorType.NotFound);

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

        if (url is null) return Result<UrlAnalyticsOutputDTO>.Failure("Url not found", ResultErrorType.NotFound);

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

        if (url is null) return Result<UpdateUrlOutputDTO>.Failure("Url not found");

        var target = string.IsNullOrEmpty(input.Target) ? url.Target : new Url(input.Target!);
        var redirection = input.TemporaryRedirection ? input.TemporaryRedirection : url.TemporaryRedirection;
        url.Update(target, redirection);

        await _repository.Update(url);
        await _cache.RemoveAsync(input.Hash);

        var result = new UpdateUrlOutputDTO(url.Hash, url.Target.Value, url.TemporaryRedirection);
        return Result<UpdateUrlOutputDTO>.Success(result);
    }
}
