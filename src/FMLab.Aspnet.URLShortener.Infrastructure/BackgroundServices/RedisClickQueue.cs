// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.DTOs;
using FMLab.Aspnet.URLShortener.Business.Services.Click;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Redis;
using StackExchange.Redis;

namespace FMLab.Aspnet.URLShortener.Infrastructure.BackgroundServices;

public class RedisClickQueue(IConnectionMultiplexer connectionMultiplexer) : IClickQueue
{
    private IDatabase Db => connectionMultiplexer.GetDatabase(RedisConstants.Databases.Queue);

    public void Enqueue(RecordClickInputDTO input)
    {
        var entries = new NameValueEntry[]
        {
            new("hash",       input.Hash      ?? string.Empty),
            new("ip",         input.IpAddress ?? string.Empty),
            new("user_agent", input.UserAgent ?? string.Empty),
            new("referer",    input.Referer   ?? string.Empty),
        };

        Db.StreamAddAsync(RedisConstants.Stream.ClickStream, entries, flags: CommandFlags.FireAndForget);
    }
}
