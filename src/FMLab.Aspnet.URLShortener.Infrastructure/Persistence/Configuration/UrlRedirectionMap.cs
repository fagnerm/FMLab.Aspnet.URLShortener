// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Configuration;
internal class UrlRedirectionMap
{
    public string Hash { get; set; }
    public string Target { get; set; }
    public bool TemporaryRedirection { get; set; }

    public UrlRedirectionMap(){}
}
