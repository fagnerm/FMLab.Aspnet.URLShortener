// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.ValueObjects;

using FMLab.Aspnet.URLShortener.Business.Entities;

namespace FMLab.Aspnet.URLShortener.Business.Repositories;
public interface IUrlRepository
{
    Task AddAsync(UrlRedirection url, CancellationToken cancellationToken);
    Task Delete(UrlRedirection url);
    Task<bool> ExistAsync(Alias alias, CancellationToken cancellationToken);
    Task<UrlRedirection?> GetByHashAsync(string id, CancellationToken cancellationToken);
    Task<UrlRedirection> Update(UrlRedirection url);
}
