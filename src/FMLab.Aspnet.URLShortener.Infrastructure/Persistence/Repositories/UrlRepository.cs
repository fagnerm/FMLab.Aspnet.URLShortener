// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Entities;
using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Business.ValueObjects;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Configuration;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Shared;
using StackExchange.Redis;
using System.Text.Json;

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Repositories;
public class UrlRepository : IUrlRepository
{
    private readonly IDatabaseAsync _db;

    public UrlRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase(RedisDatabases.URL_REDIRECTIONS);
    }

    public async Task AddAsync(UrlRedirection url, CancellationToken cancellationToken)
    {
        var mapper = new UrlRedirectionMap
        {
            Hash = url.Hash,
            Target = url.Target.Value,
            TemporaryRedirection = url.TemporaryRedirection
        };

        await _db.StringSetAsync(url.Hash, JsonSerializer.Serialize(mapper));
    }

    public async Task Delete(UrlRedirection url)
    {
        await _db.StringDeleteAsync(url.Hash, ValueCondition.Exists);
    }

    public async Task<UrlRedirection?> GetByHashAsync(string hash, CancellationToken cancellationToken)
    {
        var value = await _db.StringGetAsync(hash);

        if (value.IsNull)
            return default;

        var mapper = JsonSerializer.Deserialize<UrlRedirectionMap>(value!);

        if (mapper is null)
            return default;

        return new UrlRedirection(mapper.Hash, new Url(mapper.Target), mapper.TemporaryRedirection);
    }

    public async Task<UrlRedirection> Update(UrlRedirection url)
    {
        var mapper = new UrlRedirectionMap
        {
            Hash = url.Hash,
            Target = url.Target.Value,
            TemporaryRedirection = url.TemporaryRedirection
        };

        await _db.StringSetAsync(url.Hash, JsonSerializer.Serialize(mapper));

        return url;
    }

}
