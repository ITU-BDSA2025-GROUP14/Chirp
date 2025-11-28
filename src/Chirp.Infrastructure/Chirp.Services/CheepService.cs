using Chirp.Core.DTO;
using Chirp.Core.Repositories;

namespace Chirp.Infrastructure.Chirp.Services;

public class CheepService
{
    private readonly ICheepRepository _repository;

    public CheepService(ICheepRepository repository)
    {
        _repository = repository;
    }

    public List<CheepDto> GetCheeps()
    {
        return _repository.GetCheeps()
            .Select(ToViewModel)
            .ToList();
    }

    public List<CheepDto> GetCheeps(int pageNumber, int pageSize)
    {
        return _repository.GetCheeps(pageNumber, pageSize)
            .Select(ToViewModel)
            .ToList();
    }

    public int GetTotalCheepCount()
    {
        return _repository.GetTotalCheepCount();
    }

    public List<CheepDto> GetCheepsFromAuthor(string author)
    {
        return _repository.GetCheepsFromAuthor(author)
            .Select(ToViewModel)
            .ToList();
    }

    public List<CheepDto> GetCheepsFromFollowings(List<string> followings)
    {
        List<CheepDto> AllCheeps = new List<CheepDto>();

        foreach(var follower in followings)
        {
            AllCheeps.AddRange(GetCheepsFromAuthor(follower));
        }

        return AllCheeps;

    }
    

    public List<CheepDto> GetCheepsFromAuthor(string author, int pageNumber, int pageSize)
    {
        return _repository.GetCheepsFromAuthor(author, pageNumber, pageSize)
            .Select(ToViewModel)
            .ToList();
    }

    public int GetTotalCheepCountByAuthor(string author)
    {
        return _repository.GetTotalCheepCountByAuthor(author);
    }

    public async Task CreateCheep(string authorName, string message)
    {
        await _repository.CreateCheep(authorName, message);
    }

    private static CheepDto ToViewModel(global::Chirp.Core.Cheep cheep)
    {
        string authorName = cheep.Author?.Name ?? "Unknown";
        return new CheepDto(
            authorName,
            cheep.Text,
            cheep.TimeStamp.ToString("MM/dd/yy H:mm:ss"));
    }
}