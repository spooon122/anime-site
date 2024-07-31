namespace AnimeSite.Infrastructure;

public interface IPasswordHash
{
    string Generate(string password);

    bool Verify(string password, string hashedPassword);
}