namespace Chirp.Core.Repositories;

public interface IFollowingsRepository
{
    Task<List<String>> GetFollowing(string author);
}