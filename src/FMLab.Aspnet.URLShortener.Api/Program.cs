// API - Layered architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.DependencyInjection;
using FMLab.Aspnet.URLShortener.Infrastructure.DependencyInjection;
using FMLab.Aspnet.URLShortener.Configuration;
using FMLab.Aspnet.URLShortener.Middlewares;
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

var app = builder.Build();

app.UseAppSwagger();
app.UseApplicationEndpoints();
app.UseAppProblemDetails();
app.UseAuthentication();
app.UseAuthorization();

app.Run();