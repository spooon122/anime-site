using System.ComponentModel.DataAnnotations;

namespace anime_site.Contracts;

public record CreateAnimeRequest(
    [Required] string Name);
