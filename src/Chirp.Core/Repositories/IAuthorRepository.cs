namespace Chirp.Core.Repositories;

public interface IAuthorRepository
{
    Task<Author?> GetAuthorByName(string name);
    Task<Author> MakeSureAuthorExists(string name, string email);
}
