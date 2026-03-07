// API - Layered architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Api.Endpoints.URL;

namespace FMLab.Aspnet.URLShortener.Configuration;

public static class AppEndpoints
{
    public static WebApplication UseApplicationEndpoints(this WebApplication app)
    {
        UrlEndpoints.Map(app);

        return app;
    }
}
