using Chirp.Core.DTO;
using Chirp.Core.Repositories;

using Microsoft.AspNetCore.Components.Sections;

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

    public List<CheepDto> GetCheepsFromFollowings(List<string> followings,  string author, int pageNumber, int pageSize)
    {
        return _repository.GetCheepsFromFollowings(followings, author, pageNumber, pageSize)
            .Select(ToViewModel)
            .ToList();
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
    
    public int GetTotalCheepCountFromFollowings(List<string> followings, string author)
    {
        return _repository.GetTotalCheepCountFromFollowings(followings, author);
    }

    public async Task CreateCheep(string authorName, string message)
    {
        await _repository.CreateCheep(authorName, message);
    }

    private static CheepDto ToViewModel(global::Chirp.Core.Cheep cheep)
    {
        string authorName = cheep.Author?.Name ?? "Unknown";
        return new CheepDto(
            cheep.CheepId,
            authorName,
            cheep.Text,
            cheep.TimeStamp.ToString("MM/dd/yy H:mm:ss"),
            cheep.Likes);
    }
    
    public void LikeCheep(int cheepId)
    {
        Console.WriteLine($"Liking cheep {cheepId}");
        Console.WriteLine("Before: " + _repository.GetCheepLike(cheepId));
        _repository.UpdateCheepLike(
            cheepId,
            _repository.GetCheepLike(cheepId)+1);
        Console.WriteLine("After: " + _repository.GetCheepLike(cheepId));
    }

    public async Task<List<String>> GetFollowing(string authorName)
    {
        List<string> followings = await _repository.GetFollowing(authorName);
        return followings;
    }

    public async Task<bool> RemoveFollowing(string authorName, string targetName)
    {
        return await _repository.RemoveFollowing(authorName, targetName);
    }

    public async Task<bool> AddToFollowing(string authorName, string targetName)
    {
        return await _repository.AddToFollowing(authorName, targetName);
    }
    
}