namespace Chirp.Core.Repositories;

public interface ICheepRepository
{
    List<Cheep> GetCheeps();
    List<Cheep> GetCheeps(int pageNumber, int pageSize);
    int GetTotalCheepCount();
    List<Cheep> GetCheepsFromAuthor(string author);
    List<Cheep> GetCheepsFromAuthor(string author, int pageNumber, int pageSize);
    int GetTotalCheepCountByAuthor(string author);
    Task CreateCheep(string authorName, string message);
    public List<Cheep> GetCheepsFromFollowings(List<string> followings, string author, int pageNumber, int pageSize);
    int GetTotalCheepCountFromFollowings(List<string> followings, string author);
    Task<List<String>> GetFollowing(string authorName);
    Task<bool> AddToFollowing(string authorName, string targetName);
    Task<bool> RemoveFollowing(string authorName, string targetName);
}
