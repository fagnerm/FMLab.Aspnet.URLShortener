// API - Layered architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.URLShortener.Api.Endpoints.URL;

public record UpdateShortenURLRequest(string Target, bool TemporaryRedirection);