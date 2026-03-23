// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Api.Configuration;
using FMLab.Aspnet.URLShortener.Api.Endpoints.URL;
using FMLab.Aspnet.URLShortener.Api.Pages;
using FMLab.Aspnet.URLShortener.Authentication;
using FMLab.Aspnet.URLShortener.Business.DependencyInjection;
using FMLab.Aspnet.URLShortener.Business.Options;
using FMLab.Aspnet.URLShortener.Configuration;
using FMLab.Aspnet.URLShortener.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApplication();
builder.Services.AddAppProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAppSwagger();
builder.Services.AddAuthentication("ApiKey")
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthHandler>("ApiKey", null);
builder.Services.AddAuthorization();
builder.Services.AddRateLimiting();
builder.Services.AddAppApiVersioning();
builder.Services.AddAntiforgery();
builder.Services.Configure<AppOptions>(builder.Configuration.GetSection("UrlShortener"));
builder.AddCorsPolicy();

var app = builder.Build();

app.UseAppSwagger();
app.UseAppProblemDetails();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiting();
app.UseCorsPolicy();
app.UseAppHealthCheck();
app.MapPageEndpoints();
app.MapUrlRedirectionEndpoint();

var versionedApi = app.UseAppVersioning();
versionedApi.MapUrlEndpoints();

app.Run();