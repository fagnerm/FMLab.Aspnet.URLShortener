// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using System.Text;

namespace FMLab.Aspnet.URLShortener.Business.Shared.Encoders;
public static class Base62
{
    const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static string Encode(long value)
    {
        var pivotedChars = Pivot(chars, (int)value % 62);

        var result = new StringBuilder();
        do
        {
            var index = (int)value % 62;
            result.Append(pivotedChars[index]);
            value /= 62;

        } while (value > 0);
        return result.ToString();
    }

    private static string Pivot(string text, int value)
    {
        var chars = text.AsSpan();

        return $"{new string(chars[(value)..])}{new string(chars[..value])}";
    }
}
