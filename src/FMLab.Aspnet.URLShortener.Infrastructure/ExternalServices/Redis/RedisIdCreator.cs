// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Services.Identifier;
using FMLab.Aspnet.URLShortener.Infrastructure.Shared;
using StackExchange.Redis;

namespace FMLab.Aspnet.URLShortener.Infrastructure.ExternalServices.Redis;
public class RedisIdCreator : IIdentifierService
{
    private readonly IDatabaseAsync _db;

    public RedisIdCreator(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase(RedisDatabases.IDENTIFIERS);
    }

    public async Task<long> GetIdAsync()
    {
        await _db.StringSetAsync("url:id_seq", 100_000_000, when: When.NotExists);
        var id = await _db.StringIncrementAsync("url:id_seq");
        
        return id;
    }
}
