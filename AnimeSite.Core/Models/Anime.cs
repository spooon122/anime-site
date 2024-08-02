namespace AnimeSite.Core.Models;

public class Anime(Guid id, string name)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    
    public static Anime Create(Guid id, string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Title cannot be null!");

        return new Anime(id, name);
    }
}