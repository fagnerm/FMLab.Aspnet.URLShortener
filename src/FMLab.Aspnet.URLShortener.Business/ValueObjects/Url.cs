// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Shared.Result;
using System.Text.RegularExpressions;

namespace FMLab.Aspnet.URLShortener.Business.ValueObjects;
public record Url : IComparable<Url>
{
    public string Value { get; init; }
    public static Url Blank = new("http://about:blank");

    private Url(string url)
    {
        Value = url;
    }

    public static Result<Url> Create(string url)
    {
        if (string.IsNullOrEmpty(url))
            return Result<Url>.Failure("Must inform a Url");

        if (!IsValid(url))
            return Result<Url>.Failure("Must inform a valid Url");

        Url entity = new(url);
        return Result<Url>.Success(entity);
    }

    private static bool IsValid(string name)
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
