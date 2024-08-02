using AnimeSite.Core.Models;

namespace AnimeSite.DataAccess.Repositories;

public interface IAnimeRepository
{
    Task Create(Anime anime);
}