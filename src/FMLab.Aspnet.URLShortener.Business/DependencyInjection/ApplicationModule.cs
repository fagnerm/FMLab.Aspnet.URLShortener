// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Services.URL;
using Microsoft.Extensions.DependencyInjection;

namespace FMLab.Aspnet.URLShortener.Business.DependencyInjection;
public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUrlService, UrlService>();

        return services;
    }
}