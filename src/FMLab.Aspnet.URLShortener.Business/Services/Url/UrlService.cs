// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.DTOs;
using FMLab.Aspnet.URLShortener.Business.Entities;
using FMLab.Aspnet.URLShortener.Business.Exceptions;
using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Business.Services.Identifier;
using FMLab.Aspnet.URLShortener.Business.Shared.Result;
using FMLab.Aspnet.URLShortener.Business.ValueObjects;

namespace FMLab.Aspnet.URLShortener.Business.Services.URL;

public class UrlService(IIdentifierService idService, IUrlRepository repository) : IUrlService
{
    private readonly IUrlRepository _repository = repository;
    private readonly IIdentifierService _idService = idService;

    public async Task<Result<CreateUrlOutputDTO>> RegisterUrlAsync(CreateUrlInputDTO input, CancellationToken cancellationToken)
    {
        var target = new Url(input.Target);
        var id = await _idService.GetIdAsync();
        var url = new UrlRedirection(id, target, input.TemporaryRedirection);

        try
        {
            await _repository.AddAsync(url, cancellationToken);
        }
        catch (DomainException ex) when (ex.Message == "Url already exists")
        {
            return Result<CreateUrlOutputDTO>.Conflict(ex.Message);
        }

        var result = new CreateUrlOutputDTO(url.Hash);
        return Result<CreateUrlOutputDTO>.Success(result);
    }

    public async Task<Result<NoOutput>> DeleteUrlAsync(DeleteUrlInputDTO input, CancellationToken cancellationToken)
    {
        var existingUrl = await _repository.GetByHashAsync(input.Hash, cancellationToken);

        if (existingUrl is null) return Result<NoOutput>.NotFound("Url not found");

        await _repository.Delete(existingUrl!);

        return Result<NoOutput>.NoContent();
    }


    public async Task<Result<UrlRedirectionOutputDTO>> LoadUrlRedirection(UrlRedirectionInputDTO input, CancellationToken cancellationToken)
    {
        var url = await _repository.GetByHashAsync(input.Hash, cancellationToken);

        if (url == null)
        {
            return Result<UrlRedirectionOutputDTO>.NotFound("Url not found");
        }

        var result = new UrlRedirectionOutputDTO(url.Target.Value, url.TemporaryRedirection);
        return Result<UrlRedirectionOutputDTO>.Success(result);
    }

    public async Task<Result<UpdateUrlOutputDTO>> UpdateUrlAsync(UpdateUrlInputDTO input, CancellationToken cancellationToken)
    {
        var url = await _repository.GetByHashAsync(input.Hash, cancellationToken);

        if (url is null) return Result<UpdateUrlOutputDTO>.NotFound("User not found");

        var target = string.IsNullOrEmpty(input.Target) ? url.Target : new Url(input.Target!);
        var redirection = input.TemporaryRedirection ? input.TemporaryRedirection : url.TemporaryRedirection;
        url.Update(target, redirection);

        await _repository.Update(url);

        var result = new UpdateUrlOutputDTO(url.Hash, url.Target.Value, url.TemporaryRedirection);
        return Result<UpdateUrlOutputDTO>.Success(result);
    }
}
