// Worker - URL Shortener Click Recorder
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Entities;
using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FMLab.Aspnet.URLShortener.Worker.BackgroundServices;

public class ClickRecorderWorker(
    IConnectionMultiplexer connectionMultiplexer,
    IServiceScopeFactory scopeFactory,
    ILogger<ClickRecorderWorker> logger) : BackgroundService
{
    private const int BatchSize = 10;
    private const int BlockMs  = 5_000;

    private IDatabase Db => connectionMultiplexer.GetDatabase(RedisConstants.Databases.Queue);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await EnsureConsumerGroupAsync();
        await ProcessPendingAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await Db.StreamReadGroupAsync(
                    RedisConstants.Stream.ClickStream,
                    RedisConstants.Stream.ConsumerGroup,
                    RedisConstants.Stream.ConsumerName,
                    ">",
                    count: BatchSize,
                    noAck: false);

                if (messages is null || messages.Length == 0)
                    continue;

                await ProcessMessagesAsync(messages, stoppingToken);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading from click stream");
                await Task.Delay(2_000, stoppingToken);
            }
        }
    }

    // On startup, reprocess messages that were delivered but never acknowledged
    // (e.g. app crashed mid-batch on a previous run).
    private async Task ProcessPendingAsync(CancellationToken stoppingToken)
    {
        var pending = await Db.StreamReadGroupAsync(
            RedisConstants.Stream.ClickStream,
            RedisConstants.Stream.ConsumerGroup,
            RedisConstants.Stream.ConsumerName,
            "0",
            count: BatchSize);

        while (pending is { Length: > 0 })
        {
            await ProcessMessagesAsync(pending, stoppingToken);

            pending = await Db.StreamReadGroupAsync(
                RedisConstants.Stream.ClickStream,
                RedisConstants.Stream.ConsumerGroup,
                RedisConstants.Stream.ConsumerName,
                "0",
                count: BatchSize);
        }
    }

    private async Task ProcessMessagesAsync(StreamEntry[] messages, CancellationToken stoppingToken)
    {
        var clicks = new List<UrlClick>(messages.Length);
        var ids    = new RedisValue[messages.Length];

        for (var i = 0; i < messages.Length; i++)
        {
            ids[i]  = messages[i].Id;
            clicks.Add(Parse(messages[i]));
        }

        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<IUrlClickRepository>();

            foreach (var click in clicks)
                await repository.AddAsync(click, stoppingToken);

            await Db.StreamAcknowledgeAsync(
                RedisConstants.Stream.ClickStream,
                RedisConstants.Stream.ConsumerGroup,
                ids);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist {Count} click(s) — messages remain pending", messages.Length);
        }
    }

    private async Task EnsureConsumerGroupAsync()
    {
        try
        {
            await Db.StreamCreateConsumerGroupAsync(
                RedisConstants.Stream.ClickStream,
                RedisConstants.Stream.ConsumerGroup,
                StreamPosition.NewMessages,
                createStream: true);
        }
        catch (RedisServerException ex) when (ex.Message.Contains("BUSYGROUP"))
        {
            // Group already exists — expected on restart.
        }
    }

    private static UrlClick Parse(StreamEntry entry)
    {
        string? Get(string field) =>
            entry[field] is { IsNullOrEmpty: false } v ? v.ToString() : null;

        return new UrlClick(
            entry["hash"].ToString(),
            Get("ip"),
            Get("user_agent"),
            Get("referer"));
    }
}
