// API - Layered architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.


using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Context;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Repositories;
using FMLab.Aspnet.URLShortener.Business.ExternalServices.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using FMLab.Aspnet.URLShortener.Business.Repositories;
using StackExchange.Redis;
using FMLab.Aspnet.URLShortener.Business.Services.Identifier;
using FMLab.Aspnet.URLShortener.Infrastructure.ExternalServices.Redis;

namespace FMLab.Aspnet.URLShortener.Infrastructure.DependencyInjection;
public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, IHostEnvironment environment)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connection = new NpgsqlConnectionStringBuilder()
            {
                Host = config["Database:Server"],
                Port = int.Parse(config["Database:Port"]),
                Database = config["Database:Name"],
                Username = config["Database:User"],
                Password = config["Database:Password"],
                NoResetOnClose = true
            };

            options.UseNpgsql(connection.ConnectionString)
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging()
                       .LogTo(Console.WriteLine);
            }
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUrlRepository, UrlRepository>();

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"{config["Redis:Host"]}:{config["Redis:Port"]}"));
        services.AddSingleton<IIdentifierService, RedisIdentifierService>();

        return services;
    }
}

