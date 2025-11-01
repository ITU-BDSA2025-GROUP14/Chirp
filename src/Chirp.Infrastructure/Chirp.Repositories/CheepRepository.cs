using Chirp.Razor.Interfaces;
using Chirp.Razor.Models;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _context;

    public CheepRepository(ChirpDBContext context)
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

    private IQueryable<Cheep> BuildCheepQuery()
    {
        return _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .ThenByDescending(c => c.CheepId);
    }
}
