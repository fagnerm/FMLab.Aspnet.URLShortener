// API - Layered architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.Shared.Result;

namespace FMLab.Aspnet.URLShortener.Extensions;

internal static class ResultExtensions
{
    public static IResult ToProblemResult<TResult>(this Result<TResult> result, ResultType? @default = null)
    where TResult : class
    {
        var type = result.IsSuccess && @default.HasValue ? @default : result.Type;

        return type switch
        {
            ResultType.Success => Results.Ok(result.Data),
            ResultType.NoContent => Results.NoContent(),
            ResultType.NotFound => Results.Problem(result.Error, statusCode: StatusCodes.Status404NotFound, type: "about:blank"),
            ResultType.Validation => Results.Problem(result.Error, statusCode: StatusCodes.Status422UnprocessableEntity, type: "about:blank"),
            ResultType.Domain => Results.Problem(result.Error, statusCode: StatusCodes.Status422UnprocessableEntity, type: "about:blank"),
            ResultType.Conflict => Results.Problem(result.Error, statusCode: StatusCodes.Status409Conflict, type: "about:blank"),
            _ => Results.Problem(statusCode: 500, type: "about:blank")
        };
    }
}
