// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.DTOs;

namespace FMLab.Aspnet.URLShortener.Business.Services.Click;

public interface IClickQueue
{
    void Enqueue(RecordClickInputDTO input);
}
