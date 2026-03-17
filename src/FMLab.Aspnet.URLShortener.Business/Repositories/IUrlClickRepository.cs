// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.DTOs;
using FMLab.Aspnet.URLShortener.Business.Entities;

namespace FMLab.Aspnet.URLShortener.Business.Repositories;

public interface IUrlClickRepository
{
    Task AddAsync(UrlClick click, CancellationToken cancellationToken);
    Task<long> CountByHashAsync(string hash, CancellationToken cancellationToken);
    Task<IReadOnlyList<DailyClicksDTO>> GetDailyClicksAsync(string hash, int days, CancellationToken cancellationToken);
    Task<IReadOnlyList<TopReferrerDTO>> GetTopReferrersAsync(string hash, int top, CancellationToken cancellationToken);
}
