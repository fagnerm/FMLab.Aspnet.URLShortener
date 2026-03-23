// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.DTOs;
using FMLab.Aspnet.URLShortener.Business.Entities;
using FMLab.Aspnet.URLShortener.Business.Exceptions;
using FMLab.Aspnet.URLShortener.Business.Options;
using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Business.Services.Cache;
using FMLab.Aspnet.URLShortener.Business.Services.Click;

using FMLab.Aspnet.URLShortener.Business.Services.Identifier;
using FMLab.Aspnet.URLShortener.Business.Shared.Result;
using FMLab.Aspnet.URLShortener.Business.ValueObjects;
using Microsoft.Extensions.Options;

namespace FMLab.Aspnet.URLShortener.Business.Services.URL;
public class UrlService(IIdentifierService idService, IUrlRepository repository, IUrlClickRepository clickRepository, IUrlCacheService cache, IClickQueue clickQueue, IOptions<AppOptions> options) : IUrlService
{
    private readonly IUrlRepository _repository = repository;
    private readonly IUrlClickRepository _clickRepository = clickRepository;
    private readonly IUrlCacheService _cache = cache;
    private readonly IIdentifierService _idService = idService;
    private readonly IOptions<AppOptions> _options = options;
    private readonly IClickQueue _clickQueue = clickQueue;

    public async Task<Result<CreateUrlOutputDTO>> CreateAsync(CreateUrlInputDTO input, CancellationToken cancellationToken)
    {
        var target = Url.Create(input.Target);
        if (target.IsFailure)
            return Result<CreateUrlOutputDTO>.Failure(target.Message!);

        var id = await _idService.GetIdAsync();
        var alias = Alias.Create(input.Alias);

        if (alias.IsFailure)
            return Result<CreateUrlOutputDTO>.Failure(alias.Message!);

        var url = new UrlRedirection(id, target.Data!, input.TemporaryRedirection, alias.Data);

        try
        {
            await _repository.AddAsync(url, cancellationToken);
        }
        catch (DomainException ex)
        {
            return Result<CreateUrlOutputDTO>.Failure(ex.Message, ResultErrorType.Conflict);
        }

        var result = new CreateUrlOutputDTO($"{_options.Value.Domain}/{url.Hash}");
        return Result<CreateUrlOutputDTO>.Success(result);
    }

    public async Task<Result> DeleteAsync(DeleteUrlInputDTO input, CancellationToken cancellationToken)
    {
        var existingUrl = await _repository.GetByHashAsync(input.Hash, cancellationToken);

        if (existingUrl is null)
            return Result.Failure("Url not found", ResultErrorType.NotFound);

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

    public async Task<Result<UpdateUrlOutputDTO>> UpdateUrlAsync(UpdateUrlInputDTO input, CancellationToken cancellationToken)
    {
        var url = await _repository.GetByHashAsync(input.Hash, cancellationToken);

        if (url is null)
            return Result<UpdateUrlOutputDTO>.Failure("Url not found");

        var target = Url.Create(input.Target!);

        if (target.IsFailure)
            return Result<UpdateUrlOutputDTO>.Failure(target.Message!);

        var redirection = input.TemporaryRedirection ? input.TemporaryRedirection : url.TemporaryRedirection;
        url.Update(target.Data!, redirection);

        await _repository.Update(url);
        await _cache.RemoveAsync(input.Hash);

        var result = new UpdateUrlOutputDTO(url.Hash, url.Target.Value, url.TemporaryRedirection);
        return Result<UpdateUrlOutputDTO>.Success(result);
    }

    public async Task<Result<UrlAnalyticsOutputDTO>> GetAnalyticsAsync(UrlAnalyticsInputDTO input, CancellationToken cancellationToken)
    {
        var url = await _repository.GetByHashAsync(input.Hash, cancellationToken);

        if (url is null)
            return Result<UrlAnalyticsOutputDTO>.Failure("Url not found", ResultErrorType.NotFound);

        var totalClicks = await _clickRepository.CountByHashAsync(input.Hash, cancellationToken);
        var clicksByDay = await _clickRepository.GetDailyClicksAsync(input.Hash, AppOptions.ANALYTICS_PERIOD, cancellationToken);
        var topReferrers = await _clickRepository.GetTopReferrersAsync(input.Hash, AppOptions.TOP_REFERRERS, cancellationToken);

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

    public async Task<Result<bool>> AliasCheckerAsync(string alias, CancellationToken cancellationToken)
    {
        var result = Alias.Create(alias);
        if (result.IsFailure)
            return Result<bool>.Failure("Invalid alias");

        var aliasValue = result.Data!.Value!;

        if (await _cache.ExistAsync(aliasValue))
            return Result<bool>.Success(true);

        var url = await _repository.GetByHashAsync(aliasValue, cancellationToken);

        if (url is not null)
        {
            var dto = new UrlRedirectionOutputDTO(url.Target.Value, url.TemporaryRedirection);
            await _cache.SetAsync(aliasValue, dto);
            return Result<bool>.Success(true);
        }

        return Result<bool>.Success(false);
    }

    public void RecordClick(RecordClickInputDTO input)
    {
        _clickQueue.Enqueue(input);
    }
}
