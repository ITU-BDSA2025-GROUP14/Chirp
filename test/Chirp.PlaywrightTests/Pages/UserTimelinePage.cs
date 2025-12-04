using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Chirp.PlaywrightTests.Pages;

public class UserTimelinePage
{
    private readonly IPage _page;

    public UserTimelinePage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync(string user)
        => await _page.GotoAsync($"http://localhost:5273/UserTimeline/{user}");

    public async Task<bool> CheepExistsAsync(string message)
    {
        var content = await _page.ContentAsync();
        return content.Contains(message);
    }
}