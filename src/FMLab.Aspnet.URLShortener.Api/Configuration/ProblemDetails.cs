// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Exceptions;

namespace FMLab.Aspnet.URLShortener.Configuration;

public static class ProblemDetails
{
    public static IServiceCollection AddAppProblemDetails(this IServiceCollection services)
    {
        services.AddExceptionHandler<DomainException>();
        services.AddExceptionHandler<GenericException>();
        services.AddProblemDetails();

        return services;
    }

    public static WebApplication UseAppProblemDetails(this WebApplication app)
    {
        app.UseExceptionHandler(options =>
        {
            options.Run(async context =>
            {
                await Results.Problem()
                             .ExecuteAsync(context);
            });
        });
        app.UseStatusCodePages();

        return app;
    }
}
