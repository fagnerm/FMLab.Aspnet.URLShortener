// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.DTOs;
using FMLab.Aspnet.URLShortener.Business.Entities;
using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Context;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Views;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Repositories;

public class UrlClickRepository(ApplicationDbContext context) : IUrlClickRepository
{
    public async Task AddAsync(UrlClick click, CancellationToken cancellationToken)
    {
        await context.AddAsync(click, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<long> CountByHashAsync(string hash, CancellationToken cancellationToken)
    {
        return context.Clicks
                      .Where(c => c.Hash == hash)
                      .LongCountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DailyClicksDTO>> GetDailyClicksAsync(string hash, int days, CancellationToken cancellationToken)
    {
        var since = DateTime.UtcNow.AddDays(-days).Date;

        var result = await context
                            .Database
                            .SqlQueryRaw<DailyClicksView>("SELECT year, month, day, count FROM fn_daily_clicks({0}, {1})",hash, since)
                            .ToListAsync();

        return [.. result.Select(_ => new DailyClicksDTO(new DateOnly(_.Year, _.Month, _.Day), _.Count))];
    }

    public async Task<IReadOnlyList<TopReferrerDTO>> GetTopReferrersAsync(string hash, int top, CancellationToken cancellationToken)
    {
        var result = await context
                            .Database
                            .SqlQueryRaw<TopReferrerDTO>("SELECT referer, count FROM fn_top_referers({0},{1})", hash, top)
                            .ToListAsync();

        return result;
    }
}
