// API - Layered architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FMLab.Aspnet.URLShortener.Exceptions;

public class GenericException : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
    {
        if (exception is Business.Exceptions.DomainException) return false;

        var problem = new ProblemDetails
        {
            //Type = "about:blank",
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Error"
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
