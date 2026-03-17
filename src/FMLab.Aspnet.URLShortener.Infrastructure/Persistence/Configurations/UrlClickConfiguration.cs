// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Configurations;

public class UrlClickConfiguration : IEntityTypeConfiguration<UrlClick>
{
    public void Configure(EntityTypeBuilder<UrlClick> builder)
    {
        builder.ToTable("url_clicks");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
               .UseIdentityByDefaultColumn();
        builder.Property(c => c.Hash)
               .HasColumnType("VARCHAR(15)")
               .IsRequired();
        builder.Property(c => c.ClickedAt)
               .HasColumnType("TIMESTAMPTZ")
               .IsRequired();
        builder.Property(c => c.IpAddress)
               .HasColumnType("VARCHAR(45)");
        builder.Property(c => c.UserAgent)
               .HasColumnType("VARCHAR(512)");
        builder.Property(c => c.Referer)
               .HasColumnType("VARCHAR(2048)");
        builder.Property(c => c.Country)
               .HasColumnType("VARCHAR(2)");

        builder.HasIndex(c => c.Hash);
        builder.HasIndex(c => c.ClickedAt);
    }
}
