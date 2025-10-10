using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using SQLitePCL;

namespace Chirp.Razor.Pages;
public class PublicModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int page { get; set; } = 1;
    public int PageCount { get; set; }
    public int PageSize { get; set; } = 32;

    public int TotalPages => (int)Math.Ceiling(decimal.Divide(PageCount, PageSize));

    public List<CheepViewModel> Cheeps { get; set; }


    private readonly CheepService _service;

    public PublicModel(CheepService service)
    {
        _service = service;
    }
    public bool ShowPrevious => page > 1;
    public bool ShowNext => page < TotalPages;
    public ActionResult OnGet()
    {
        PageCount = _service.GetTotalCheepCount();
        Cheeps = _service.GetCheeps(page, PageSize);
        return Page();
    }
}
