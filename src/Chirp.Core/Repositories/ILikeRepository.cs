namespace Chirp.Core.Repositories;

public interface ILikeRepository
{
    Task<bool> AddLike(int cheepId, string authorName);
    Task<bool> RemoveLike(int cheepId, string authorName);
    Task<bool> HasUserLiked(int cheepId, string authorName);
    Task<int> GetLikeCount(int cheepId);
    Task<Dictionary<int, int>> GetLikeCounts(IEnumerable<int> cheepIds);
    Task<HashSet<int>> GetLikedCheepIds(string authorName, IEnumerable<int> cheepIds);
}
