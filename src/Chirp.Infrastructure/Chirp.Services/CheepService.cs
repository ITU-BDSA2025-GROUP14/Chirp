using System.Data;

using Chirp.Razor;
using Microsoft.EntityFrameworkCore;

public record CheepViewModel(string Author, string Message, string Timestamp);

public class CheepService
{
    private readonly ChirpDBContext _context;

    public CheepService(ChirpDBContext context)
    {
        _context = context;
    }

    public List<CheepViewModel> GetCheeps()
    {
        return _context.Cheeps.Include(c => c.Author).OrderByDescending(c => c.TimeStamp).Select(c =>
                new CheepViewModel(
                    c.Author.Name,
                    c.Text,
                    c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
            .ToList();
    }

    public List<CheepViewModel> GetCheeps(int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(pageNumber, 1);
        int skip = (pageNumber - 1) * pageSize;

        return _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .ThenByDescending(c => c.CheepId)  // <- tie-breaker
            .Skip(skip)
            .Take(pageSize)
            .Select(c => new CheepViewModel(
                c.Author.Name,
                c.Text,
                c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
            .ToList();
    }

    public int GetTotalCheepCount()
    {
        return _context.Cheeps.Count();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name using SQL query
        return _context.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == author)
            .OrderByDescending(c => c.TimeStamp)
            .Select(c => new CheepViewModel(
                c.Author.Name,
                c.Text,
                c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
            .ToList();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber, int pageSize)
    {
        int skip = (pageNumber - 1) * pageSize;

        return _context.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == author)
            .OrderByDescending(c => c.TimeStamp)
            .ThenByDescending(c => c.CheepId)
            .Skip(skip)
            .Take(pageSize)
            .Select(c => new CheepViewModel(
                c.Author.Name,
                c.Text,
                c.TimeStamp.ToString("MM/dd/yy H:mm:ss")))
            .ToList();
    }


    public int GetTotalCheepCountByAuthor(string author)
    {
        return _context.Cheeps
            .Include(c => c.Author)
            .Count(c => c.Author.Name == author);
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}
