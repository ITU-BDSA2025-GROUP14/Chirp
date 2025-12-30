using Chirp.Core.DTO;
using Chirp.Core.Repositories;

namespace Chirp.Infrastructure.Chirp.Services;

public class CheepService
{
    private readonly ICheepRepository _repository;
    private readonly ILikeRepository _likeRepository;
    private string? _currentUserName;

    public CheepService(ICheepRepository repository, ILikeRepository likeRepository)
    {
        _repository = repository;
        _likeRepository = likeRepository;
    }

    public void SetCurrentUser(string? userName)
    {
        _currentUserName = userName;
    }

    public List<CheepDto> GetCheeps()
    {
        return _repository.GetCheeps()
            .Select(ToViewModel)
            .ToList();
    }

    public async Task<List<CheepDto>> GetCheepsAsync(int pageNumber, int pageSize)
    {
        var cheeps = _repository.GetCheeps(pageNumber, pageSize);
        return await EnrichWithLikeDataAsync(cheeps);
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

    public async Task<List<CheepDto>> GetCheepsFromFollowingsAsync(List<string> followings, string author, int pageNumber, int pageSize)
    {
        var cheeps = _repository.GetCheepsFromFollowings(followings, author, pageNumber, pageSize);
        return await EnrichWithLikeDataAsync(cheeps);
    }

    public async Task<List<CheepDto>> GetCheepsFromAuthorAsync(string author, int pageNumber, int pageSize)
    {
        var cheeps = _repository.GetCheepsFromAuthor(author, pageNumber, pageSize);
        return await EnrichWithLikeDataAsync(cheeps);
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

    public async Task<(bool hasLiked, int likeCount)> ToggleLikeAsync(int cheepId, string authorName)
    {
        var hasLiked = await _likeRepository.HasUserLiked(cheepId, authorName);

        if (hasLiked)
        {
            await _likeRepository.RemoveLike(cheepId, authorName);
        }
        else
        {
            await _likeRepository.AddLike(cheepId, authorName);
        }

        var likeCount = await _likeRepository.GetLikeCount(cheepId);
        return (!hasLiked, likeCount);
    }

    private async Task<List<CheepDto>> EnrichWithLikeDataAsync(List<global::Chirp.Core.Cheep> cheeps)
    {
        if (cheeps.Count == 0)
        {
            return new List<CheepDto>();
        }

        var cheepIds = cheeps.Select(c => c.CheepId).ToList();

        // batch fetching the like counts for all cheeps
        var likeCounts = await _likeRepository.GetLikeCounts(cheepIds);

        // and batch fetching user'ss likes if the user is authenticated
        HashSet<int> userLikedCheepIds = new HashSet<int>();
        if (!string.IsNullOrEmpty(_currentUserName))
        {
            userLikedCheepIds = await _likeRepository.GetLikedCheepIds(_currentUserName, cheepIds);
        }

        return cheeps.Select(cheep =>
        {
            string authorName = cheep.Author?.Name ?? "Unknown";
            int likeCount = likeCounts.TryGetValue(cheep.CheepId, out var count) ? count : 0;
            bool hasLiked = userLikedCheepIds.Contains(cheep.CheepId);

            return new CheepDto(
                cheep.CheepId,
                authorName,
                cheep.Text,
                cheep.TimeStamp.ToString("MM/dd/yy H:mm:ss"),
                likeCount,
                hasLiked
            );
        }).ToList();
    }

    private static CheepDto ToViewModel(global::Chirp.Core.Cheep cheep)
    {
        string authorName = cheep.Author?.Name ?? "Unknown";
        return new CheepDto(
            cheep.CheepId,
            authorName,
            cheep.Text,
            cheep.TimeStamp.ToString("MM/dd/yy H:mm:ss"),
            0,     // LikeCount (we use async methods for like data
            false  // HasLiked (same here... we use async methods for like data)
        );
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
