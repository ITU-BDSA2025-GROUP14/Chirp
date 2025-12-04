using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Chirp.PlaywrightTests.Pages;

public class PublicTimelinePage
{
    private readonly IPage _page;

    public PublicTimelinePage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync()
        => await _page.GotoAsync("http://localhost:5273/");

    public async Task PostCheepAsync(string message)
    {
        await _page.FillAsync("#Text", message);
        await _page.RunAndWaitForNavigationAsync(async () =>
        {
            await _page.ClickAsync("input[type=submit]");
        });

    }

    public async Task<bool> CheepExistsAsync(string message)
    {
        var content = await _page.ContentAsync();
        return content.Contains(message);
    }
}