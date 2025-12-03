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

    public List<Cheep> GetCheepsFromFollowings(List<string> followings, string author, int pageNumber, int pageSize){
        pageNumber = Math.Max(pageNumber, 1);
        int skip = (pageNumber - 1) * pageSize;
        
        var authorNames = new List<string>(followings) { author };
        authorNames.Add(author);
        return BuildCheepQuery()
            .Where(c => authorNames.Contains(c.Author.Name))
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
                $"Author '{authorName}' does not exist. Author has to be created during login/registration before posting cheeps.");
        }

        // creating cheep
        var cheep = new Cheep
        {
            Author = author,
            AuthorId = author.AuthorId,
            Text = message,
            TimeStamp = DateTime.UtcNow,
            Likes = 0
        };

        _context.Cheeps.Add(cheep);
        await _context.SaveChangesAsync();
    }
    
    public int GetTotalCheepCountFromFollowings(List<string> followings, string author)
    {
        var authorNames = new List<string>(followings) { author };
    
        return _context.Cheeps
            .Include(c => c.Author)
            .Count(c => authorNames.Contains(c.Author.Name));
    }
    
    public async Task<List<String>> GetFollowing(string authorName)
    {
        //get the current author
        var author = await _context.Authors
            .FirstOrDefaultAsync(a => a.Name == authorName);

        if (author == null)
        {
            return new List<String>();
        }
        //get the users that the current author follows
        return author.followings.ToList();
    }
    
    public async Task<bool> AddToFollowing(string authorName, string targetName)
    {
        var author = await _context.Authors
            .FirstOrDefaultAsync(a  => a.Name == authorName );
        
        var target = await _context.Authors
            .FirstOrDefaultAsync(a => a.Name == targetName);
        
        
        if (author == null || target == null)
        {
            return false; 
        }

        if (!author.followings.Contains(targetName))
        {
            author.followings.Add(targetName);
            await _context.SaveChangesAsync();
        }

        return true;
    }
    
    public async Task<bool> RemoveFollowing(string authorName, string targetName)
    {
        var author = await _context.Authors
            .FirstOrDefaultAsync(a  => a.Name == authorName);
        var  target = await _context.Authors
            .FirstOrDefaultAsync(a => a.Name == targetName);
        
        if (author == null || target == null)
        {
            return false;
        }

        if (author.followings.Contains(targetName))
        {
            author.followings.Remove(targetName);
            await _context.SaveChangesAsync();
        }
        return true;
    }
    
    private IQueryable<Cheep> BuildCheepQuery()
    {
        return _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .ThenByDescending(c => c.CheepId);
    }
    public int GetCheepLike(int CheepId)
    {
        Cheep? cheep = _context.Cheeps.Find(CheepId);
        return cheep.Likes;
    }

    public void UpdateCheepLike(int CheepId, int likes)
    {
        Cheep? cheep = _context.Cheeps.Find(CheepId);
        cheep.Likes = likes;
        _context.SaveChanges();
    }
}
