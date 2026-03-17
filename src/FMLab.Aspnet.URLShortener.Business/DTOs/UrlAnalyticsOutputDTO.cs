// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.URLShortener.Business.DTOs;

public record UrlAnalyticsOutputDTO(
    string Hash,
    string Target,
    DateTime CreatedAt,
    long TotalClicks,
    IReadOnlyList<DailyClicksDTO> ClicksByDay,
    IReadOnlyList<TopReferrerDTO> TopReferrers
);

public record DailyClicksDTO(DateOnly Date, int Count);
public record TopReferrerDTO(string? Referer, int Count);
