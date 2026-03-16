// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.URLShortener.Business.DTOs;
using FMLab.Aspnet.URLShortener.Business.Services.URL;
using FMLab.Aspnet.URLShortener.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FMLab.Aspnet.URLShortener.Api.Endpoints.URL;

public static class UrlEndpoints
{
    public static void MapUrlRedirectionEndpoint(this WebApplication app)
    {
        app.MapGet("/{hash}", RedirecToUrlEndpoint)
            .WithTags("Url")
            .Produces(StatusCodes.Status307TemporaryRedirect)
            .ProducesValidationProblem(StatusCodes.Status404NotFound);
    }

    public static RouteGroupBuilder MapUrlEdnpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/url", RegisterUrlEndpoint)
            .WithTags("Url")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status409Conflict)
            .RequireAuthorization()
            .MapToApiVersion(1, 0);

        group.MapPatch("/{hash}", UpdateUrl)
            .WithTags("Url")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization()
            .MapToApiVersion(1, 0);

        group.MapDelete("/{hash}", DeleteUrlEndpoint)
            .WithTags("Url")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .MapToApiVersion(1, 0);

        return group;
    }

    private static async Task<IResult> RedirecToUrlEndpoint([FromServices] IUrlService service, [FromRoute] string hash, CancellationToken cancellationToken)
    {
        var input = new UrlRedirectionInputDTO(hash);
        var output = await service.LoadUrlRedirection(input, cancellationToken);

        if (!output.IsSuccess) return output.ToProblemResult();

        return Results.Redirect(output.Data!.Target, output.Data.TemporaryRedirection);
    }

    private static async Task<IResult> RegisterUrlEndpoint([FromServices] IUrlService service, [FromBody] CreateShortenURLRequest body, CancellationToken cancellationToken)
    {
        var input = new CreateUrlInputDTO(body.Target, body.TemporaryRedirection);
        var output = await service.RegisterUrlAsync(input, cancellationToken);

        if (!output.IsSuccess) return output.ToProblemResult();

        var result = output.Data;
        return Results.Created(result!.UrlShortened, result);
    }

    private static async Task<IResult> UpdateUrl([FromServices] IUrlService service, [FromRoute] string hash, [FromBody] UpdateShortenURLRequest body, CancellationToken cancellationToken)
    {
        var input = new UpdateUrlInputDTO(hash, body.Target, body.TemporaryRedirection);
        var output = await service.UpdateUrlAsync(input, cancellationToken);

        return output.ToProblemResult();
    }

    private static async Task<IResult> DeleteUrlEndpoint([FromServices] IUrlService service, [FromRoute] string hash, CancellationToken cancellationToken)
    {
        var input = new DeleteUrlInputDTO(hash);
        var output = await service.DeleteUrlAsync(input, cancellationToken);

        return output.ToProblemResult();
    }
}
