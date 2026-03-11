// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Services.Identifier;
using StackExchange.Redis;

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Redis;
public class RedisIdentifierService : IIdentifierService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisIdentifierService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<long> GetIdAsync()
    {
        var db = _connectionMultiplexer.GetDatabase();
        await db.StringSetAsync("url:id_seq", 100_000_000, when: When.NotExists);

        var id = await db.StringIncrementAsync("url:id_seq");
        return id;
    }
}
