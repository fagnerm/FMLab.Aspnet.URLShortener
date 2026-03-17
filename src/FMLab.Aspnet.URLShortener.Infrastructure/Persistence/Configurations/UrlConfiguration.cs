// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Entities;
using FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Configurations;
public class UrlConfiguration : IEntityTypeConfiguration<UrlRedirection>
{
    public void Configure(EntityTypeBuilder<UrlRedirection> builder)
    {
        builder.ToTable("short_urls");
        builder.HasKey(_ => _.Hash);
        builder.Property(c => c.Hash)
               .HasColumnType("VARCHAR(15)")
               .IsRequired();
        builder.Property(n => n.Target)
               .HasConversion<UrlToStringConverter>()
               .HasColumnType("VARCHAR(2048)")
               .IsRequired();
        builder.Property(s => s.TemporaryRedirection)
               .HasColumnType("BOOL")
               .HasColumnName("Temporary")
               .IsRequired();
        builder.Property(s => s.IsActive)
               .HasColumnType("BOOL")
               .HasDefaultValue(true)
               .IsRequired();
        builder.Property(s => s.CreatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .IsRequired();
        builder.Property(s => s.UpdatedAt)
               .HasColumnType("TIMESTAMPTZ");
        builder.Property(s => s.ExpiresAt)
               .HasColumnType("TIMESTAMPTZ");
        builder.Property(s => s.MaxClicks)
               .HasColumnType("INT");
        builder.HasIndex(u => u.Hash).IsUnique();
    }
}
