// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Shared.Result;
using System.Text.RegularExpressions;

namespace FMLab.Aspnet.URLShortener.Business.ValueObjects;
public record Alias : IComparable<Alias>
{
    public string? Value { get; init; }

    private Alias(string alias)
    {
        Value = alias;
    }

    public static Result<Alias> Create(string? alias)
    {
        if (string.IsNullOrEmpty(alias))
        {
            return Result<Alias>.Success(default!);
        }

        if (!IsValid(alias!))
        {
            return Result<Alias>.Failure("Alias must contain only letters, numbers and hyphens, up to 15 characters.");
        }

        Alias entity = new(alias!);
        return Result<Alias>.Success(entity);
    }

    private static bool IsValid(string alias)
    {
        var pattern = @"^[a-zA-Z0-9\-]{1,15}$";
        return Regex.IsMatch(alias, pattern);

    }
    public int CompareTo(Alias? other)
    {
        return string.Compare(Value, other?.Value, StringComparison.Ordinal);
    }

    public override string ToString() => string.IsNullOrEmpty(Value) ? string.Empty : Value.ToString();
}
