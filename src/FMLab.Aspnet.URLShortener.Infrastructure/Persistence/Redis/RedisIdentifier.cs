// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Options;
using FMLab.Aspnet.URLShortener.Business.Services.Identifier;
using StackExchange.Redis;

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Redis;
public class RedisIdentifier : IIdentifierService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisIdentifier(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<long> GetIdAsync()
    {
        var db = _connectionMultiplexer.GetDatabase(RedisConstants.Databases.Sequence);
        await db.StringSetAsync(RedisConstants.Keys.IdSequence, AppOptions.IDENTIFIER_SEED, when: When.NotExists);

        var id = await db.StringIncrementAsync(RedisConstants.Keys.IdSequence);
        return id;
    }
}
