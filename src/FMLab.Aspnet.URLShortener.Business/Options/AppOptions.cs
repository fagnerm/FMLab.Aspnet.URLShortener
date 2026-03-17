// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.


namespace FMLab.Aspnet.URLShortener.Business.Options;
public class AppOptions
{
    public const long IDENTIFIER_SEED = 100_000_000;
    public const int ANALYTICS_PERIOD = 30;
    public const int TOP_REFERRERS = 5;

    public string? Domain { get; set; }
}
