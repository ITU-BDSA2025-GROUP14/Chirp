using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using Chirp.Core.DTO;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Chirp.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;
public class PublicModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    public int PageCount { get; set; }
    public int PageSize { get; set; } = 32;

    public int TotalPages => (int)Math.Ceiling(decimal.Divide(PageCount, PageSize));

    public List<CheepDto> Cheeps { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Enter a message, please")]
    [StringLength(160, ErrorMessage = "The message can not exceed 160 chars")]
    public string Text { get; set; } = string.Empty;

    private readonly CheepService _service;
    private readonly IAuthorRepository _authorRepository;
    private readonly IFollowingsRepository _followingRepository;

    public PublicModel(CheepService service, IAuthorRepository authorRepository, IFollowingsRepository followingsRepository)
    {
        _service = service;
        _authorRepository = authorRepository;
        _followingRepository = followingsRepository;
    }
    public bool ShowPrevious => CurrentPage > 1;
    public bool ShowNext => CurrentPage < TotalPages;
    public List<string>? FollowingList { get; set; } = new List<string>();
    
    public async Task<ActionResult> OnGetAsync([FromQuery] int page = 1)
    {
        page = Math.Max(page, 1);
        CurrentPage = page;

        PageCount = _service.GetTotalCheepCount();
        Cheeps = _service.GetCheeps(CurrentPage, PageSize);

        // Load following list for authenticated users
        if (User.Identity?.IsAuthenticated == true)
        {
            var authorName = User.Identity.Name;
            if (!string.IsNullOrEmpty(authorName))
            {
                FollowingList = await _followingRepository.GetFollowing(authorName);
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // reloadng the page with validation errors
            PageCount = _service.GetTotalCheepCount();
            Cheeps = _service.GetCheeps(CurrentPage, PageSize);
            
            if (User.Identity?.IsAuthenticated == true)
            {
                var userName = User.Identity.Name;
                if (!string.IsNullOrEmpty(userName))
                {
                    FollowingList = await _followingRepository.GetFollowing(userName);
                }
            }
            return Page();
        }

        // getting authenticated users name
        var authorName = User.Identity?.Name;
        if (string.IsNullOrEmpty(authorName))
        {
            ModelState.AddModelError(string.Empty, "You must be logged in to post a cheep");
            PageCount = _service.GetTotalCheepCount();
            Cheeps = _service.GetCheeps(CurrentPage, PageSize);
            return Page();
        }

        // getting users email
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? $"{authorName}@chirp.dk";

        // makinng sure that author exist in db before creating cheep
        await _authorRepository.MakeSureAuthorExists(authorName, userEmail);

        // creating the cheep
        await _service.CreateCheep(authorName, Text);
        
        // redirecting to same page in order to prevent resubmissions
        return RedirectToPage("/Public", new { page = CurrentPage });
    }

    public async Task<IActionResult> OnPostFollowAsync(string targetName)
    {
        var authorName = User.Identity?.Name;
        if (string.IsNullOrEmpty(authorName))
        {
            return RedirectToPage("/Public", new { page = CurrentPage });
        }
        
        await _followingRepository.AddToFollowing(authorName, targetName);
        return RedirectToPage("/Public", new { page = CurrentPage });
    }

    public async Task<IActionResult> OnPostUnfollowAsync(string targetName)
    {
        var authorName = User.Identity?.Name;
        if (string.IsNullOrEmpty(authorName))
        {
            return RedirectToPage("/Public", new { page = CurrentPage });
        }
       await _followingRepository.RemoveFollowing(authorName, targetName);
        return RedirectToPage("/Public", new { page = CurrentPage });
    }

}
