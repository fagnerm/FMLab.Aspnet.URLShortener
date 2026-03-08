// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.DTOs;
using FMLab.Aspnet.URLShortener.Business.Shared.Result;

namespace FMLab.Aspnet.URLShortener.Business.Services.URL;

public interface IUrlService
{
    Task<Result<UrlRedirectionOutputDTO>> LoadUrlRedirection(UrlRedirectionInputDTO input, CancellationToken cancellationToken);
    Task<Result<CreateUrlOutputDTO>> RegisterUrlAsync(CreateUrlInputDTO input, CancellationToken cancellationToken);
    Task<Result<UpdateUrlOutputDTO>> UpdateUrlAsync(UpdateUrlInputDTO input, CancellationToken cancellationToken);
    Task<Result<NoOutput>> DeleteUrlAsync(DeleteUrlInputDTO input, CancellationToken cancellationToken);
}