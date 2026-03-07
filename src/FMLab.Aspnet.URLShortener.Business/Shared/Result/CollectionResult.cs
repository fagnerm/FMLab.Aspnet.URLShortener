// API - Layered architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.URLShortener.Business.Shared.Result;
public record CollectionResult<TItems>
    where TItems : class
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; }
    public IReadOnlyCollection<TItems> Items { get; init; }

    public CollectionResult(IReadOnlyCollection<TItems> items, int page, int pageSize, int totalItems)
    {
        Items = items ??= [];
        Page = page;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }
}
