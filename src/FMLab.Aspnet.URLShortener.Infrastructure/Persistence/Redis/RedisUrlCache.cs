// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.DTOs;
using FMLab.Aspnet.URLShortener.Business.Options;
using FMLab.Aspnet.URLShortener.Business.Services.Cache;
using StackExchange.Redis;
using System.Text.Json;

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Redis;

public class RedisUrlCache(IConnectionMultiplexer connectionMultiplexer) : IUrlCacheService
{
    private IDatabase Db => connectionMultiplexer.GetDatabase(RedisConstants.Databases.Cache);

    public async Task<UrlRedirectionOutputDTO?> GetAsync(string hash)
    {
        var value = await Db.StringGetAsync(RedisConstants.Keys.UrlRedirect(hash));

        if (value.IsNullOrEmpty) return null;

        return JsonSerializer.Deserialize<UrlRedirectionOutputDTO>(value!);
    }

    public async Task SetAsync(string hash, UrlRedirectionOutputDTO dto)
    {
        var value = JsonSerializer.Serialize(dto);
        await Db.StringSetAsync(RedisConstants.Keys.UrlRedirect(hash), value, TimeSpan.FromHours(AppOptions.CACHE_TTL_HOURS));
    }

    public async Task RemoveAsync(string hash)
    {
        await Db.KeyDeleteAsync(RedisConstants.Keys.UrlRedirect(hash));
    }

    public async Task<bool> ExistAsync(string alias)
    {
        var found = await Db.StringGetAsync(RedisConstants.Keys.UrlRedirect(alias));

        return found.HasValue;
    }
}
