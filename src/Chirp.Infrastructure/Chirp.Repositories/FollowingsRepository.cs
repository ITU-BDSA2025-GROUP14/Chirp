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
    public List<Author> GetFollowing()
    {
        return BuildAuthorQuery().ToList();
    }

    public async Task AddFollowingAsync(int authorId, int followingId)
    {
        var author = await _context.Authors
            .Include(a => a.followings)
            .FirstOrDefaultAsync(a  => a.AuthorId == authorId);
        
        
        var following = await _context.Authors.FindAsync(followingId);

        if (author != null && following != null && !author.followings.Contains(following))
        {
            author.followings.Add(following);
            await _context.SaveChangesAsync();  
        }
    }

    public Task RemoveFollowing()
    {
        throw new NotImplementedException();
    }

    public IQueryable<Author> BuildAuthorQuery()
    {
        return _context.Authors.Include(f => f.followings);
    }
}