// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.


using Asp.Versioning;
using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Business.Services.Identifier;
using FMLab.Aspnet.URLShortener.Infrastructure.ExternalServices.Redis;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Context;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using StackExchange.Redis;
using System.Threading.RateLimiting;

namespace FMLab.Aspnet.URLShortener.Infrastructure.DependencyInjection;
public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, IHostEnvironment environment)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connection = new NpgsqlConnectionStringBuilder()
            {
                Host = config["Database:Server"],
                Port = int.Parse(config["Database:Port"]!),
                Database = config["Database:Name"],
                Username = config["Database:User"],
                Password = config["Database:Password"],
                NoResetOnClose = true
            };

            options.UseNpgsql(connection.ConnectionString, npgsql =>
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

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue)
                    ? retryAfterValue
                    : TimeSpan.FromMinutes(1);

                context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();
                context.HttpContext.Response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Too Many Requests",
                    Detail = "Request limit exceeded. Please try again later.",
                    Instance = context.HttpContext.Request.Path
                };

                await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            };

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    }));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUrlRepository, UrlRepository>();

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"{config["Redis:Host"]}:{config["Redis:Port"]}"));
        services.AddSingleton<IIdentifierService, RedisIdentifierService>();

        return services;
    }

    public static RouteGroupBuilder UseAppVersioning(this WebApplication app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1, 0))
                            .ReportApiVersions()
                            .Build();

        var versionedApi = app.MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(versionSet);

        return versionedApi;
    }
}

