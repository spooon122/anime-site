using AnimeSite.Core.Models;
using AnimeSite.DataAccess.Repositories;

namespace AnimeSite.Application.Services;

public class AnimeService : IAnimeService
{
    private readonly IAnimeRepository _animeRepository;

    public AnimeService(IAnimeRepository animeRepository)
    {
        _animeRepository = animeRepository;
    }

    public async Task CreateAnime(Anime anime)
    {
        await _animeRepository.Create(anime);
    }
}