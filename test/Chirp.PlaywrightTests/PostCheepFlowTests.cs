using Chirp.PlaywrightTests.Pages;
using Xunit;

namespace Chirp.PlaywrightTests;

public class PostCheepFlowTests : PlaywrightTestBase
{
    [Fact]
    public async Task User_CanPostCheep_AndSeeItInTimeline()
    {
        var login = new LoginPage(Page);
        var publicPage = new PublicTimelinePage(Page);

        await login.NavigateAsync();
        await login.LoginAsync("ropf@itu.dk", "LetM31n!");

        await publicPage.NavigateAsync();

        var message = "Playwright test cheep!";
        await publicPage.PostCheepAsync(message);

        Assert.True(await publicPage.CheepExistsAsync(message));
    }
}