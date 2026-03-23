// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.


using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Business.Services.Cache;
using FMLab.Aspnet.URLShortener.Business.Services.Identifier;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Context;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Redis;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Repositories;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using StackExchange.Redis;

namespace FMLab.Aspnet.URLShortener.Infrastructure.DependencyInjection;
public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, IHostEnvironment environment)
    {
        var postgresConnection = new NpgsqlConnectionStringBuilder()
        {
            Host = config["Database:Server"],
            Port = int.Parse(config["Database:Port"]!),
            Database = config["Database:Name"],
            Username = config["Database:User"],
            Password = config["Database:Password"],
            NoResetOnClose = true
        }
        .ConnectionString;

        var redisConnection = ConnectionMultiplexer.Connect($"{config["Redis:Host"]}:{config["Redis:Port"]}");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(postgresConnection, npgsql =>
                       npgsql.EnableRetryOnFailure(
                           maxRetryCount: 3,
                           maxRetryDelay: TimeSpan.FromSeconds(5),
                           errorCodesToAdd: null))
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging()
                       .LogTo(Console.WriteLine);
            }
        });

        services.AddScoped<IUrlRepository, UrlRepository>();
        services.AddScoped<IUrlClickRepository, UrlClickRepository>();

        services.AddSingleton<IConnectionMultiplexer>(redisConnection);
        services.AddSingleton<IIdentifierService, RedisIdentifier>();
        services.AddSingleton<IUrlCacheService, RedisUrlCache>();

        services.AddHealthChecks()
                .AddRedis(redisConnection, name: "redis")
                .AddNpgSql(postgresConnection, name: "postgres");

        return services;
    }

    public static WebApplication UseAppHealthCheck(this WebApplication app)
    {
        app.UseHealthChecks("/api/health", new HealthCheckOptions()
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponseNoExceptionDetails
        });

        return app;
    }
}

