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
        // finding the author -- it has to already exist from authentication
        var author = await _context.Authors
            .FirstOrDefaultAsync(a => a.Name == authorName);

        if (author == null)
        {
            throw new InvalidOperationException(
                $"Author '{authorName}' doesn't exist. Author has to be created during login/registration before posting cheeps.");
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
