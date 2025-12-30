using Chirp.Core;
using Chirp.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly ChirpDbContext _context;

    public LikeRepository(ChirpDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddLike(int cheepId, string authorName)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        if (author == null) return false;

        var existingLike = await _context.Likes
            .FirstOrDefaultAsync(l => l.CheepId == cheepId && l.AuthorId == author.AuthorId);

        if (existingLike != null) return false;

        var like = new Like
        {
            CheepId = cheepId,
            AuthorId = author.AuthorId,
            TimeStamp = DateTime.UtcNow
        };

        _context.Likes.Add(like);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveLike(int cheepId, string authorName)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        if (author == null) return false;

        var like = await _context.Likes
            .FirstOrDefaultAsync(l => l.CheepId == cheepId && l.AuthorId == author.AuthorId);

        if (like == null) return false;

        _context.Likes.Remove(like);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasUserLiked(int cheepId, string authorName)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        if (author == null) return false;

        return await _context.Likes
            .AnyAsync(l => l.CheepId == cheepId && l.AuthorId == author.AuthorId);
    }

    public async Task<int> GetLikeCount(int cheepId)
    {
        return await _context.Likes.CountAsync(l => l.CheepId == cheepId);
    }

    public async Task<Dictionary<int, int>> GetLikeCounts(IEnumerable<int> cheepIds)
    {
        var cheepIdList = cheepIds.ToList();

        var counts = await _context.Likes
            .Where(l => cheepIdList.Contains(l.CheepId))
            .GroupBy(l => l.CheepId)
            .Select(g => new { CheepId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CheepId, x => x.Count);

        foreach (var id in cheepIdList)
        {
            if (!counts.ContainsKey(id))
                counts[id] = 0;
        }

        return counts;
    }

    public async Task<HashSet<int>> GetLikedCheepIds(string authorName, IEnumerable<int> cheepIds)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        if (author == null) return new HashSet<int>();

        var cheepIdList = cheepIds.ToList();

        var likedIds = await _context.Likes
            .Where(l => l.AuthorId == author.AuthorId && cheepIdList.Contains(l.CheepId))
            .Select(l => l.CheepId)
            .ToListAsync();

        return new HashSet<int>(likedIds);
    }
}
