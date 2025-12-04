using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Chirp.PlaywrightTests.Pages;

public class LoginPage
{
    private readonly IPage _page;

    public LoginPage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync()
        => await _page.GotoAsync("http://localhost:5273/Identity/Account/Login");

    public async Task LoginAsync(string email, string password)
    {
        await _page.FillAsync("#Input_Email", email);
        await _page.FillAsync("#Input_Password", password);
        await _page.ClickAsync("button[type=submit]");
    }
}