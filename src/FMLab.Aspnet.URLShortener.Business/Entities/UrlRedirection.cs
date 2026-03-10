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
        Target = Url.Empty;
        Hash = string.Empty;
    }

    public UrlRedirection(long id, Url url, bool temporaryRedirection)
    {
        Hash = Base62.Encode(id);
        Target = url;
        TemporaryRedirection = temporaryRedirection;
    }

    public Url Target { get; private set; }
    public bool TemporaryRedirection { get; private set; }
    public string Hash { get; init; }


    public void Update(Url url, bool temporaryRedirection)
    {
        Target = url;
        TemporaryRedirection = temporaryRedirection;
    }
}
