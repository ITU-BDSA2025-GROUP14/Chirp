using Chirp.Core;
using Chirp.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDbContext _context;

    public AuthorRepository(ChirpDbContext context)
    {
        _context = context;
    }

    public async Task<Author?> GetAuthorByName(string name)
    {
        return await _context.Authors
            .FirstOrDefaultAsync(a => a.Name == name);
    }

    public async Task<Author> MakeSureAuthorExists(string name, string email)
    {
        var existingAuthor = await GetAuthorByName(name);

        if (existingAuthor != null)
        {
            return existingAuthor;
        }

        // creating new author if it does not exist
        var newAuthor = new Author
        {
            Name = name,
            Email = email,
            Cheeps = new List<Cheep>()
        };

        _context.Authors.Add(newAuthor);
        await _context.SaveChangesAsync();

        return newAuthor;
    }
}
