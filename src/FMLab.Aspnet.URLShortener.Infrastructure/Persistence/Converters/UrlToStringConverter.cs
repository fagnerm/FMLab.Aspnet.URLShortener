// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Converters;
public class UrlToStringConverter : ValueConverter<Url, string>
{
    public UrlToStringConverter() :
        base(n => n.Value,
             n => Url.Create(n).Data!)
    {
    }
}
