using Chirp.PlaywrightTests.Pages;
using Xunit;

namespace Chirp.PlaywrightTests.SecurityTests;

public class XssPreventionTests : PlaywrightTestBase
{
    [Theory]
    [InlineData("<script>alert('xss')</script>")]
    [InlineData("<script>document.location='http://evil.com'</script>")]
    public async Task ScriptTag_InCheepText_RendersAsEscapedText(string xssPayload)
    {
        var login = new LoginPage(Page);
        var publicPage = new PublicTimelinePage(Page);

        await login.NavigateAsync();
        await login.LoginAsync("ropf@itu.dk", "LetM31n!");

        await publicPage.NavigateAsync();
        await publicPage.PostCheepAsync(xssPayload);

        // checking that the script tag is there as text
        var pageContent = await Page.ContentAsync();

        // the payload should be there as html entities in the sourcee
        Assert.Contains("&lt;script&gt;", pageContent);
        Assert.Contains("&lt;/script&gt;", pageContent);

        // checking that there is no alert dialog that is triggered by checkingthat the page is still functional
        var dialogTriggered = false;
        Page.Dialog += (_, _) => dialogTriggered = true;
        await Page.WaitForTimeoutAsync(500);
        Assert.False(dialogTriggered, "XSS payload is executed , and alert dialog has been triggerrd");
    }

    [Theory]
    [InlineData("<img src=x onerror=\"alert('xss')\">")]
    [InlineData("<img src=\"invalid\" onerror=\"document.body.innerHTML='hacked'\">")]
    public async Task ImgTagWithOnerror_InCheepText_RendersAsEscapedText(string xssPayload)
    {
        var login = new LoginPage(Page);
        var publicPage = new PublicTimelinePage(Page);

        await login.NavigateAsync();
        await login.LoginAsync("ropf@itu.dk", "LetM31n!");

        await publicPage.NavigateAsync();
        await publicPage.PostCheepAsync(xssPayload);

        var pageContent = await Page.ContentAsync();

        // the img tag should be escaped rather than shown as html
        Assert.Contains("&lt;img", pageContent);
        Assert.Contains("onerror=", pageContent);

        // checking that the page content has not beenreplaced by XSS
        Assert.Contains("Public Timeline", pageContent);
    }

    [Theory]
    [InlineData("<a href=\"javascript:alert('xss')\">click me</a>")]
    [InlineData("<a href=\"javascript:document.location='http://evil.com'\">link</a>")]
    public async Task JavaScriptUrl_InCheepText_RendersAsEscapedText(string xssPayload)
    {
        var login = new LoginPage(Page);
        var publicPage = new PublicTimelinePage(Page);

        await login.NavigateAsync();
        await login.LoginAsync("ropf@itu.dk", "LetM31n!");

        await publicPage.NavigateAsync();
        await publicPage.PostCheepAsync(xssPayload);

        var pageContent = await Page.ContentAsync();

        // tag should be escaped ......
        Assert.Contains("&lt;a href=", pageContent);

        // checking that there is not any link that is clickable with javascript
        var jsLinks = await Page.Locator("a[href^='javascript:']").CountAsync();
        Assert.Equal(0, jsLinks);
    }

    [Theory]
    [InlineData("<div onmouseover=\"alert('xss')\">hover me</div>")]
    [InlineData("<body onload=\"alert('xss')\">")]
    public async Task EventHandlerAttributes_InCheepText_RendersAsEscapedText(string xssPayload)
    {
        var login = new LoginPage(Page);
        var publicPage = new PublicTimelinePage(Page);

        await login.NavigateAsync();
        await login.LoginAsync("ropf@itu.dk", "LetM31n!");

        await publicPage.NavigateAsync();
        await publicPage.PostCheepAsync(xssPayload);

        var pageContent = await Page.ContentAsync();

        // evemnt handler tags should be escaped
        Assert.Contains("&lt;", pageContent);

        // checking that there isnt any elements withwith  event handlers which are created from user inputs
        var cheepList = Page.Locator("#messagelist");
        var dangerousElements = await cheepList.Locator("[onmouseover], [onload], [onclick], [onerror]").CountAsync();
        Assert.Equal(0, dangerousElements);
    }
}
