using AnimeSite.Core.Models;

namespace AnimeSite.Application.Services;

public interface IAnimeService
{
    Task CreateAnime(Anime anime);
}