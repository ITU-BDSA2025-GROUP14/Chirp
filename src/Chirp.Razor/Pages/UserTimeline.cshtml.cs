using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    public int PageCount { get; set; }
    public int PageSize { get; set; } = 32;
    public string Author { get; set; }
    
    
    
    public int TotalPages => (int)Math.Ceiling(decimal.Divide(PageCount, PageSize));
    
    private readonly CheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(CheepService service)
    {
        _service = service;
    }
    
    public bool ShowPrevious => CurrentPage > 1;
    public bool ShowNext => CurrentPage < TotalPages;
    public ActionResult OnGet(string author)
    {
        Author = author;
        Cheeps = _service.GetCheepsFromAuthor(author);
        PageCount = Cheeps.Count;
        Cheeps = Cheeps.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        return Page();
    }
}