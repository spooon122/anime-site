using AnimeSite.Core.Abstractions;
using AnimeSite.Core.Models;
using AnimeSite.Infrastructure;

namespace AnimeSite.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHash _passwordHash;
    private readonly IJwtProvider _jwtProvider;

    public UserService(IUserRepository userRepository, IPasswordHash passwordHash, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHash = passwordHash;
        _jwtProvider = jwtProvider;
    }
    public async Task Register(string userName, string email, string password)
    {
        var hashedPassword = _passwordHash.Generate(password);

        var user = User.CreateUser(Guid.NewGuid(), userName, hashedPassword, email);

        await _userRepository.Add(user);
    }

    public async Task<string> Login(string email, string password)
    {
        var user = await _userRepository.GetByEmail(email);

        var result = _passwordHash.Verify(password, user.PasswordHash);

        if (result == false)
        {
            throw new Exception("Failed to login");
        }

        var token = _jwtProvider.GenerateToken(user);
        return token;
    }
}