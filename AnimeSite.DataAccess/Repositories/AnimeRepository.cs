using AnimeSite.Core.Models;
using AnimeSite.DataAccess.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AnimeSite.DataAccess.Repositories;

public class AnimeRepository : IAnimeRepository
{
    private readonly AnimeDbContext _context;

    public AnimeRepository(AnimeDbContext context)
    {
        _context = context;
    }

    public async Task Create(Anime anime)
    {
        var animeEntity = new AnimeEntity()
        {
            Id = anime.Id,
            Name = anime.Name
        };

        await _context.Anime.AddAsync(animeEntity);
        await _context.SaveChangesAsync();
    }
}