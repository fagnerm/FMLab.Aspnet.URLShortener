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
               .HasColumnName("hash")
               .IsRequired();
        builder.Property(n => n.Target)
               .HasConversion<UrlToStringConverter>()
               .HasColumnType("VARCHAR(2048)")
               .HasColumnName("target")
               .IsRequired();
        builder.Property(s => s.TemporaryRedirection)
               .HasColumnType("BOOL")
               .HasColumnName("temporary")
               .IsRequired();
        builder.Property(s => s.IsActive)
               .HasColumnType("BOOL")
               .HasColumnName("is_active")
               .HasDefaultValue(true)
               .IsRequired();
        builder.Property(s => s.CreatedAt)
               .HasColumnType("timestamp with time zone")
               .HasColumnName("created_at")
               .IsRequired();
        builder.Property(s => s.UpdatedAt)
               .HasColumnName("updated_at")
               .HasColumnType("timestamp with time zone");
        builder.Property(s => s.ExpiresAt)
               .HasColumnName("expires_at")
               .HasColumnType("timestamp with time zone");
        builder.Property(s => s.MaxClicks)
               .HasColumnName("max_clicks")
               .HasColumnType("INT");
        builder.HasIndex(u => u.Hash).IsUnique();
    }
}
