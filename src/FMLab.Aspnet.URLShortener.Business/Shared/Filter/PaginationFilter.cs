// API - Layered architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.URLShortener.Business.Shared.Filter;
public record PaginationFilter
{
    private const int MAX_PAGE_SIZE = 100;

    public int Page { get; init; }
    public int PageSize { get; init; }

    public PaginationFilter(int? page, int? pageSize)
    {
        pageSize ??= 100;

        Page = page ?? 1;
        PageSize = Math.Min((int)pageSize, MAX_PAGE_SIZE);
    }
}