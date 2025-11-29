using Chirp.Core;

using Chirp.Core.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

public class FollowingsRepository : IFollowingsRepository
{
    private readonly ChirpDbContext _context;

    public FollowingsRepository(ChirpDbContext context)
    {
        _context = context;
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

    public IQueryable<Author> BuildAuthorQuery()
    {
        return _context.Authors.Include(f => f.followings);
    }
}