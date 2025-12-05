using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using System.Threading.Tasks;

namespace Chirp.PlaywrightTests;

public class PlaywrightTestBase : IAsyncLifetime
{
    protected IPlaywright Playwright;
    protected IBrowser Browser;
    protected IPage Page;

    public async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new() { Headless = true });
        var context = await Browser.NewContextAsync();
        Page = await context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await Browser.CloseAsync();
        Playwright.Dispose();
    }
}