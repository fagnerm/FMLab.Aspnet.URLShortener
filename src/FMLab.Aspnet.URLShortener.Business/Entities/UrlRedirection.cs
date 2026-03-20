// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Shared.Encoders;
using FMLab.Aspnet.URLShortener.Business.ValueObjects;

namespace FMLab.Aspnet.URLShortener.Business.Entities;

public class UrlRedirection
{
    private UrlRedirection()
    {
        Target = Url.Blank;
        Hash = string.Empty;
    }

    public UrlRedirection(long id, Url url, bool temporaryRedirection, Alias? alias = null, DateTime? expiresAt = null, int? maxClicks = null)
    {
        Hash = alias is null ? Base62.Encode(id) : alias.Value!;
        Target = url;
        TemporaryRedirection = temporaryRedirection;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        MaxClicks = maxClicks;
        IsActive = true;
    }

    public string Hash { get; init; }
    public Url Target { get; private set; }
    public bool TemporaryRedirection { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public int? MaxClicks { get; private set; }

    public void Update(Url url, bool temporaryRedirection)
    {
        Target = url;
        TemporaryRedirection = temporaryRedirection;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
