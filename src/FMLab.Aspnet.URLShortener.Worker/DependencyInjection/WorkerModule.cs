// Worker - URL Shortener Click Recorder
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Worker.BackgroundServices;
using FMLab.Aspnet.URLShortener.Infrastructure.DependencyInjection;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Repositories;
using StackExchange.Redis;

namespace FMLab.Aspnet.URLShortener.Worker.DependencyInjection;

public static class WorkerModule
{
    public static IServiceCollection AddWorkerInfrastructure(this IServiceCollection services, IConfiguration config, IHostEnvironment environment)
    {
        services.AddAppDbContext(config, environment);

        services.AddScoped<IUrlClickRepository, UrlClickRepository>();

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"{config["Redis:Host"]}:{config["Redis:Port"]}"));

        services.AddHostedService<ClickRecorderWorker>();

        return services;
    }
}
