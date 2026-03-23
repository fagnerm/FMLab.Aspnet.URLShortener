// Worker - URL Shortener Click Recorder
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Worker.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWorkerInfrastructure(builder.Configuration, builder.Environment);

var host = builder.Build();
host.Run();
