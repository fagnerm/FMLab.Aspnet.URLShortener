// API - Layered architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Services.Encoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMLab.Aspnet.URLShortener.Tests;
public class Test
{
    [Theory]
    [InlineData(10000000)]
    [InlineData(10000001)]
    [InlineData(10000002)]
    public void TestMethod(long entry)
    {
        var result = Base62.Encode(entry);
    }
}
