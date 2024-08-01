using AnimeSite.Core.Models;

namespace AnimeSite.Core.Abstractions;

public interface IUserRepository
{
    Task Add(User user);
    Task<User> GetByEmail(string email);
}