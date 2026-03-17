// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Entities;
using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Context;

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Repositories;

public class UrlClickRepository(ApplicationDbContext context) : IUrlClickRepository
{
    public async Task AddAsync(UrlClick click, CancellationToken cancellationToken)
    {
        await context.AddAsync(click, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
