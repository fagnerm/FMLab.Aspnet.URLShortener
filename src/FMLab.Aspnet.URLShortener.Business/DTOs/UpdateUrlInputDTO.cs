// API - Layered architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.URLShortener.Business.DTOs;
public record UpdateUrlInputDTO(string Hash, string Target, bool TemporaryRedirection);