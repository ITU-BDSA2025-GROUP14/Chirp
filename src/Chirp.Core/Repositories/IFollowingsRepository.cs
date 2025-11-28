namespace Chirp.Core.Repositories;

public interface IFollowingsRepository
{
    List<Author> GetFollowing();
}