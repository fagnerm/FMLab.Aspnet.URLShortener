// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Views;
public class DailyClicksView
{
    public  int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int Count { get; set; }
}
