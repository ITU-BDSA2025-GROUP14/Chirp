using System.Security.Claims;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Chirp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LikesController : ControllerBase
{
    private readonly CheepService _cheepService;
    private readonly IAuthorRepository _authorRepository;

    public LikesController(CheepService cheepService, IAuthorRepository authorRepository)
    {
        _cheepService = cheepService;
        _authorRepository = authorRepository;
    }

    [HttpPost("{cheepId}")]
    public async Task<IActionResult> ToggleLike(int cheepId)
    {
        var authorName = User.Identity?.Name;
        if (string.IsNullOrEmpty(authorName))
        {
            return Unauthorized();
        }

        // making sure that the author exists :D
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? $"{authorName}@chirp.dk";
        await _authorRepository.MakeSureAuthorExists(authorName, userEmail);

        var (hasLiked, likeCount) = await _cheepService.ToggleLikeAsync(cheepId, authorName);

        return Ok(new { hasLiked, likeCount });
    }
}
