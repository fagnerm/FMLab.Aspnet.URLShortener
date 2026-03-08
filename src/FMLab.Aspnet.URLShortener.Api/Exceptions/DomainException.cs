// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FMLab.Aspnet.URLShortener.Exceptions;

internal class DomainException : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not Business.Exceptions.DomainException) return false;

        var problem = new ProblemDetails
        {
            //Type = "about:blank",
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Validation",
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
