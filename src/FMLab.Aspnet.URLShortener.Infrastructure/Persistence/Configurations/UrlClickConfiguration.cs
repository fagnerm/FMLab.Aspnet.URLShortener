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
               .HasColumnName("id")
               .UseIdentityByDefaultColumn();
        builder.Property(c => c.Hash)
               .HasColumnType("VARCHAR(15)")
               .HasColumnName("hash")
               .IsRequired();
        builder.Property(c => c.ClickedAt)
               .HasColumnName("clicked_at")
               .HasColumnType("timestamp with time zone")
               .IsRequired();
        builder.Property(c => c.IpAddress)
               .HasColumnName("ip_address")
               .HasColumnType("VARCHAR(45)");
        builder.Property(c => c.UserAgent)
               .HasColumnName("user_agent")
               .HasColumnType("VARCHAR(512)");
        builder.Property(c => c.Referer)
               .HasColumnName("referer")
               .HasColumnType("VARCHAR(2048)");
        builder.Property(c => c.Country)
               .HasColumnName("country")
               .HasColumnType("VARCHAR(2)");

        builder.HasIndex(c => c.Hash);
        builder.HasIndex(c => c.ClickedAt);
    }
}
