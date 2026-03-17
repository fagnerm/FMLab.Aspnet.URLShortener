// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Entities;
using FMLab.Aspnet.URLShortener.Business.Exceptions;
using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Repositories;
public class UrlRepository : IUrlRepository
{
    private readonly ApplicationDbContext _context;

    public UrlRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UrlRedirection url, CancellationToken cancellationToken)
    {
        try
        {
            await _context.AddAsync(url, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            _context.Entry(url).State = EntityState.Detached;
            throw new DomainException("Url already exists", ex);
        }
    }

    public async Task Delete(UrlRedirection user)
    {
        _context.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<UrlRedirection?> GetByHashAsync(string hash, CancellationToken cancellationToken)
    {
        var user = await _context
                            .Urls
                            .AsTracking()
                            .SingleOrDefaultAsync(_ => _.Hash == hash, cancellationToken);

        return user;
    }

    public async Task<UrlRedirection> Update(UrlRedirection url)
    {
        var entry = _context.Update(url);
        await _context.SaveChangesAsync();

        return entry.Entity;
    }

}
