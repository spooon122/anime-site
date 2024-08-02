using AnimeSite.Core.Models;
using AnimeSite.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnimeSite.DataAccess;

public class AnimeDbContext(DbContextOptions<AnimeDbContext> options) : DbContext(options)
{
    public DbSet<AnimeEntity> Anime { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnimeDbContext).Assembly);
    }
}