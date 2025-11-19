using System.ComponentModel.DataAnnotations;

using Chirp.Core.DTO;
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

    public PublicModel(CheepService service)
    {
        _service = service;
    }
    public bool ShowPrevious => CurrentPage > 1;
    public bool ShowNext => CurrentPage < TotalPages;
    public ActionResult OnGet([FromQuery] int page = 1)
    {
        page = Math.Max(page, 1);
        CurrentPage = page;

        PageCount = _service.GetTotalCheepCount();
        Cheeps = _service.GetCheeps(CurrentPage, PageSize);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // reloadng the page with validation errors
            PageCount = _service.GetTotalCheepCount();
            Cheeps = _service.GetCheeps(CurrentPage, PageSize);
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

        // creating the cheep
        await _service.CreateCheep(authorName, Text);

        // redirecting to same page in order to prevent resubmissions
        return RedirectToPage("/Public", new { page = CurrentPage });
    }

}
