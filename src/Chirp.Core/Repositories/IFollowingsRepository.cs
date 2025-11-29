namespace Chirp.Core.Repositories;

public interface IFollowingsRepository
{
    Task<List<String>> GetFollowing(string author);
    Task<bool> RemoveFollowing(string author, string target);
    Task<bool> AddToFollowing(string author, string target);
}