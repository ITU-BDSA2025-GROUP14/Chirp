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
}
