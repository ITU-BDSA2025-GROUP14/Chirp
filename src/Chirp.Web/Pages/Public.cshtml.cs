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

    public List<CheepViewModel> Cheeps { get; set; }
    
    private readonly CheepService _service;

    public PublicModel(CheepService service)
    {
        _service = service;
    }
    public bool ShowPrevious => CurrentPage > 1;
    public bool ShowNext => CurrentPage < TotalPages;
    public ActionResult OnGet([FromQuery] int page = 1)
    {
        CurrentPage = page;
        
        PageCount = _service.GetTotalCheepCount();
        Cheeps = _service.GetCheeps(CurrentPage, PageSize);
        return Page();
    }
}
