// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Shared.Result;

namespace FMLab.Aspnet.URLShortener.Extensions;

internal static class ResultExtensions
{
    public static IResult ToProblemResult<TResult>(this Result<TResult> result)
    where TResult : class
    {
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ResultErrorType.NotFound => Results.Problem(result.Message, statusCode: StatusCodes.Status404NotFound),
                ResultErrorType.Confict => Results.Problem(result.Message, statusCode:StatusCodes.Status409Conflict),
                _ => Results.Problem(statusCode: StatusCodes.Status400BadRequest),
            };
        };

        return Results.Ok(result.Data);
    }

    public static IResult ToProblemResult(this Result result)
    {
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ResultErrorType.NotFound => Results.Problem(result.Message, statusCode: StatusCodes.Status404NotFound),
                ResultErrorType.Confict => Results.Problem(result.Message, statusCode: StatusCodes.Status409Conflict),
                _ => Results.Problem(statusCode: StatusCodes.Status400BadRequest),
            };
        };

        return Results.Ok();
    }
}
