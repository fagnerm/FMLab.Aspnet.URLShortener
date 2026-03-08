// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Exceptions;
using System.Text.RegularExpressions;

namespace FMLab.Aspnet.URLShortener.Business.ValueObjects;
public record Url : IComparable<Url>
{
    public string Value { get; init; }

    public Url(string url)
    {
        url.ThrowIfNullOrEmpty("Must inform a Url");

        if (!IsValid(url))
        {
            DomainGuard.Throw("Must inform a valid Url");
        }

        Value = url;
    }

    private bool IsValid(string name)
    {
        var pattern = @"^(http|https)://";
        return Regex.IsMatch(name, pattern);

    }
    public int CompareTo(Url? other)
    {
        return string.Compare(Value, other?.Value, StringComparison.Ordinal);
    }

    public override string ToString() => Value.ToString();
}
