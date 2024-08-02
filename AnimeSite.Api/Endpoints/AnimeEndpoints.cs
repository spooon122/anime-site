using anime_site.Contracts;
using AnimeSite.Application.Services;
using AnimeSite.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace anime_site.Endpoints;

public static class AnimeEndpoints
{
    public static IEndpointRouteBuilder MapAnimeEndpoits(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("Anime").RequireAuthorization();

        endpoints.MapPost(string.Empty, CreateAnime);

        return endpoints;
    }

    private static async Task<IResult> CreateAnime(
        [FromBody] CreateAnimeRequest request, 
        HttpContext context,
        AnimeService animeService)
    {
        var anime = Anime.Create(
            Guid.NewGuid(),
            request.Name);
        await animeService.CreateAnime(anime);

        return Results.Ok();
    }
}