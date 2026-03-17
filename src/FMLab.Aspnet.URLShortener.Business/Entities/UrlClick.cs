// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.URLShortener.Business.Entities;

public class UrlClick
{
    private UrlClick() { }

    public UrlClick(string hash, string? ipAddress, string? userAgent, string? referer)
    {
        Hash = hash;
        ClickedAt = DateTime.UtcNow;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Referer = referer;
    }

    public long Id { get; private set; }
    public string Hash { get; private set; } = string.Empty;
    public DateTime ClickedAt { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Referer { get; private set; }
    public string? Country { get; private set; }
}
