// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.


using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Business.Services.Identifier;
using FMLab.Aspnet.URLShortener.Infrastructure.ExternalServices.Redis;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace FMLab.Aspnet.URLShortener.Infrastructure.DependencyInjection;
public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, IHostEnvironment environment)
    {
        services.AddScoped<IUrlRepository, UrlRepository>();

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"{config["Redis:Host"]}:{config["Redis:Port"]}"));
        services.AddSingleton<IIdentifierService, RedisIdCreator>();

        return services;
    }
}

