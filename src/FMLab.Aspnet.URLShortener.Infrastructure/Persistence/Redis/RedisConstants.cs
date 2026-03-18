// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Redis;

public static class RedisConstants
{
    public static class Databases
    {
        public const int Cache = 0;
        public const int Sequence = 1;
    }

    public static class Keys
    {
        public const string IdSequence = "url:id_seq";
        public static string UrlRedirect(string hash) => $"url:redirect:{hash}";
    }
}
