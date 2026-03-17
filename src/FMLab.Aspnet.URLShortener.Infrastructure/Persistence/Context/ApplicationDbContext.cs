// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Entities;

using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UrlConfiguration).Assembly);
    }

    public DbSet<UrlRedirection> Urls { get; set; }
    public DbSet<UrlClick> Clicks { get; set; }
}