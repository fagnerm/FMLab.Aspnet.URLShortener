// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using System.Text.Json;
using System.Text.Json.Serialization;
using FMLab.Aspnet.URLShortener.Business.DTOs;
using FMLab.Aspnet.URLShortener.Business.Services.URL;
using Microsoft.AspNetCore.Antiforgery;

namespace FMLab.Aspnet.URLShortener.Api.Pages;

public static class PageEndpoints
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static WebApplication MapPageEndpoints(this WebApplication app)
    {
        app.MapGet("/", GetIndexPage)
            .ExcludeFromDescription();
        app.MapPost("/", PostIndexPage)
            .ExcludeFromDescription();
        app.MapGet("/analytics/{hash}", GetAnalyticsPage)
            .ExcludeFromDescription();

        return app;
    }

    private static async Task<IResult> GetIndexPage(HttpContext ctx, IAntiforgery antiforgery)
    {
        var token = antiforgery.GetAndStoreTokens(ctx);
        var html = File.ReadAllText("wwwroot/index.html")
            .Replace("{ANTIFORGERY_FIELD_NAME}", token.FormFieldName)
            .Replace("{ANTIFORGERY_TOKEN}", token.RequestToken);

        return await Task.FromResult(Results.Content(html, "text/html"));
    }

    private static async Task<IResult> GetAnalyticsPage(string hash, IUrlService urlService, CancellationToken cancellationToken)
    {
        var input = new UrlAnalyticsInputDTO(hash);
        var result = await urlService.GetAnalyticsAsync(input, cancellationToken);

        if (!result.IsSuccess) return Results.NotFound();

        var json = JsonSerializer.Serialize(result.Data, _jsonOptions);
        var html = File.ReadAllText("wwwroot/analytics.html")
            .Replace("{ANALYTICS_JSON}", json);

        return Results.Content(html, "text/html");
    }

    private static async Task<IResult> PostIndexPage(HttpContext ctx,
        IAntiforgery antiforgery,
        IUrlService urlService,
        CancellationToken cancellationToken)
    {
        await antiforgery.ValidateRequestAsync(ctx);

        var form = await ctx.Request.ReadFormAsync(cancellationToken);
        var originalUrl = form["url"].ToString();

        var input = new CreateUrlInputDTO(originalUrl, false);
        var result = await urlService.RegisterUrlAsync(input, cancellationToken);


        var html = File.ReadAllText("wwwroot/result.html")
                       .Replace("{ORIGINAL_URL}", originalUrl)
                       .Replace("{SHORT_URL}", result.Data!.UrlShortened);

        return Results.Content(html, "text/html");
    }
}
