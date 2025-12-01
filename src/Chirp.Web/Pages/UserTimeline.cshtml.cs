using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Chirp.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly CheepService _service;
    private readonly IAuthorRepository _authorRepository;

    public List<CheepDto> Cheeps { get; set; }

    [BindProperty(SupportsGet = true)]
    public int page { get; set; } = 1;
    public int PageCount { get; set; }
    public int PageSize { get; set; } = 32;
    public string Author { get; set; }
    public List<CheepDto>  FollowerCheeps { get; set; }
        
    
   
    [BindProperty]
    [Required(ErrorMessage = "Enter a message, please")]
    [StringLength(160, ErrorMessage = "The message can not exceed 160 chars")]
    public string Text { get; set; } = string.Empty;

    public int TotalPages => (int)Math.Ceiling(decimal.Divide(PageCount, PageSize));

    public UserTimelineModel(CheepService service, IAuthorRepository authorRepository)
    {
        _service = service;
        _authorRepository = authorRepository;
    }

    public bool ShowPrevious => page > 1;
    public bool ShowNext => page < TotalPages;
    public List<string>? FollowingList { get; set; } = new List<string>();
    public async Task<ActionResult> OnGetAsync(string author)
    {
        Author = author;
        PageCount = _service.GetTotalCheepCountFromFollowings(await _service.GetFollowing(author), author);
        Cheeps =  _service.GetCheepsFromFollowings(await _service.GetFollowing(author), author, page, PageSize);
        
        if (User.Identity?.IsAuthenticated == true)
        {
            var authorName = User.Identity.Name;
            if (!string.IsNullOrEmpty(authorName))
            {
                FollowingList = await _service.GetFollowing(authorName);
            }
        }
    
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string author)
    {
        Author = author;

        if (!ModelState.IsValid)
        {
            // reloading page with validaition errsors
            PageCount = _service.GetTotalCheepCountFromFollowings(await _service.GetFollowing(author), author);
            Cheeps =  _service.GetCheepsFromFollowings(await _service.GetFollowing(author), author, page, PageSize);
            return Page();
        }

        // getting authenticated users name
        var authorName = User.Identity?.Name;
        if (string.IsNullOrEmpty(authorName))
        {
            ModelState.AddModelError(string.Empty, "You must be logged in to post a cheep");
            PageCount = _service.GetTotalCheepCountFromFollowings(await _service.GetFollowing(author), author);
            Cheeps =  _service.GetCheepsFromFollowings(await _service.GetFollowing(author), author, page, PageSize);
            return Page();
        }

        // making sure thatt the author exists (normally created at login)
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? $"{authorName}@chirp.dk";
        await _authorRepository.MakeSureAuthorExists(authorName, userEmail);

        // create the cheep
        await _service.CreateCheep(authorName, Text);
        
        // redirecting to same page so that we prevent resubmission
        return RedirectToPage("/UserTimeline", new { author = author, page = page });
    }
    
    
    public async Task<IActionResult> OnPostFollowAsync(string author, string targetName)
    {
        var authorName = User.Identity?.Name;
        // reloading page if no valid user
        if (string.IsNullOrEmpty(authorName))
        {
            return RedirectToPage("/UserTimeline", new { author = author, page = page });
        }
        
        await _service.AddToFollowing(authorName, targetName);
        FollowingList = await _service.GetFollowing(authorName);
        return RedirectToPage("/UserTimeline", new { author = author, page = page });
    }

    public async Task<IActionResult> OnPostUnfollowAsync(string author, string targetName)
    {
        var authorName = User.Identity?.Name;
        if (string.IsNullOrEmpty(authorName))
        {
            return RedirectToPage("/UserTimeline", new { author = author, page = page });
        }
        await _service.RemoveFollowing(authorName, targetName);
        FollowingList = await _service.GetFollowing(authorName);
        return RedirectToPage("/UserTimeline", new { author = author, page = page });
    }
}
