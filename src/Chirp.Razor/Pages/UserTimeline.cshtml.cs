using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly CheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    [BindProperty(SupportsGet = true)]
    public int page { get; set; } = 1;
    public int PageCount { get; set; }
    public int PageSize { get; set; } = 32;
    public string Author { get; set; }

    public int TotalPages => (int)Math.Ceiling(decimal.Divide(PageCount, PageSize));

    public UserTimelineModel(CheepService service)
    {
        _service = service;
    }

    public bool ShowPrevious => page > 1;
    public bool ShowNext => page < TotalPages;

    public ActionResult OnGet(string author)
    {
        Author = author;
        var allCheeps = _service.GetCheepsFromAuthor(author);
        PageCount = allCheeps.Count;
        Cheeps = allCheeps.Skip((page - 1) * PageSize).Take(PageSize).ToList();
        return Page();
    }
}