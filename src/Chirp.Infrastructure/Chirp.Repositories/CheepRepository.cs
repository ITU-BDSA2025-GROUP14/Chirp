using Chirp.Core;
using Chirp.Core.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDbContext _context;

    public CheepRepository(ChirpDbContext context)
    {
        _context = context;
    }

    public List<Cheep> GetCheeps()
    {
        return BuildCheepQuery()
            .ToList();
    }

    public List<Cheep> GetCheeps(int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(pageNumber, 1);
        int skip = (pageNumber - 1) * pageSize;

        return BuildCheepQuery()
            .Skip(skip)
            .Take(pageSize)
            .ToList();
    }

    public int GetTotalCheepCount()
    {
        return _context.Cheeps.Count();
    }

    public List<Cheep> GetCheepsFromAuthor(string author)
    {
        return BuildCheepQuery()
            .Where(c => c.Author.Name == author)
            .ToList();
    }

    public List<Cheep> GetCheepsFromAuthor(string author, int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(pageNumber, 1);
        int skip = (pageNumber - 1) * pageSize;

        return BuildCheepQuery()
            .Where(c => c.Author.Name == author)
            .Skip(skip)
            .Take(pageSize)
            .ToList();
    }

    public int GetTotalCheepCountByAuthor(string author)
    {
        return _context.Cheeps
            .Include(c => c.Author)
            .Count(c => c.Author.Name == author);
    }

    public async Task CreateCheep(string authorName, string message)
    {
        // finding OR creating the author
        var author = await _context.Authors
            .FirstOrDefaultAsync(a => a.Name == authorName);

        if (author == null)
        {
            // creating new author if it does not exist
            author = new Author
            {
                Name = authorName,
                Email = $"{authorName}@chirp.dk", // default email
                Cheeps = new List<Cheep>()
            };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync(); // saving to get AuthorId
        }

        // creating cheep
        var cheep = new Cheep
        {
            Author = author,
            AuthorId = author.AuthorId,
            Text = message,
            TimeStamp = DateTime.UtcNow
        };

        _context.Cheeps.Add(cheep);
        await _context.SaveChangesAsync();
    }

    private IQueryable<Cheep> BuildCheepQuery()
    {
        return _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .ThenByDescending(c => c.CheepId);
    }
}
