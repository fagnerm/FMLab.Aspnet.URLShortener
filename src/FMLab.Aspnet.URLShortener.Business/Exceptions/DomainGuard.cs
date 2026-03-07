// API - Layered architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.URLShortener.Business.Exceptions;
public static class DomainGuard
{
    public static void Throw(string message, Exception? innerException = null)
    {
        throw new DomainException(message, innerException);
    }

    public static void ThrowIfNullOrEmpty(this string obj, string message)
    {
        if (!string.IsNullOrEmpty(obj)) return;

        throw new DomainException(message);
    }
}
