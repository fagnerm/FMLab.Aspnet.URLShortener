// API - Layered architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Shared.Encoders;

namespace FMLab.Aspnet.URLShortener.Tests;

public class Base62EncoderTests
{
    [Theory]
    [InlineData(10000000)]
    [InlineData(10000001)]
    [InlineData(10000002)]
    public void Encode_ReturnsNonEmptyString(long entry)
    {
        var result = Base62.Encode(entry);
    }
}
