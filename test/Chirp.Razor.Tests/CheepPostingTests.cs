using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Chirp.Services;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

public class CheepPostingTests
{
    private CheepService CreateCheepServiceWithFreshDatabase()
    {
        var context = TestDbContextFactory.CreateContext(Guid.NewGuid().ToString());
        var repository = new CheepRepository(context);
        return new CheepService(repository);
    }

    private void SetupPageContextWithAuthenticatedUser(PageModel pageModel, string username)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username)
        }, "TestAuthentication");

        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        pageModel.PageContext = new PageContext
        {
            HttpContext = httpContext
        };
    }

    private void SetupPageContextWithUnauthenticatedUser(PageModel pageModel)
    {
        var identity = new ClaimsIdentity(); // if there is no auth type, then it is not authenticated
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        pageModel.PageContext = new PageContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task PostingCheep_WhenUserIsNotLoggedIn_ShowsErrorMessageAndDoesNotCreateCheep()
    {
        var service = CreateCheepServiceWithFreshDatabase();
        var publicPageModel = new PublicModel(service)
        {
            Text = "test cheep test cheep"
        };
        SetupPageContextWithUnauthenticatedUser(publicPageModel);

        var result = await publicPageModel.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.False(publicPageModel.ModelState.IsValid);
        Assert.True(publicPageModel.ModelState.ContainsKey(string.Empty));
        var errorMessage = publicPageModel.ModelState[string.Empty]?.Errors.FirstOrDefault();
        Assert.NotNull(errorMessage);
        Assert.Equal("You must be logged in to post a cheep", errorMessage.ErrorMessage);
    }

    [Fact]
    public async Task PostingCheep_WhenUserIsAuthenticated_SuccessfullyCreatesCheepAndRedirectsToPublicTimeline()
    {
        var service = CreateCheepServiceWithFreshDatabase();
        var publicPageModel = new PublicModel(service)
        {
            Text = "test -- valid test cheep"
        };
        SetupPageContextWithAuthenticatedUser(publicPageModel, "TestUser");

        var result = await publicPageModel.OnPostAsync();

        Assert.IsType<RedirectToPageResult>(result);
        var redirectResult = result as RedirectToPageResult;
        Assert.Equal("/Public", redirectResult?.PageName);

        // verifying that the cheep is actually stored in the db
        var allCheeps = service.GetCheeps(1, 32);
        Assert.Single(allCheeps);
        Assert.Equal("test -- valid test cheep", allCheeps[0].Message);
        Assert.Equal("TestUser", allCheeps[0].Author);
    }

    [Fact]
    public async Task PostingCheep_WhenMessageExceeds160Characters_ShowsValidationErrorAndDoesNotCreateCheep()
    {
        var service = CreateCheepServiceWithFreshDatabase();
        var publicPageModel = new PublicModel(service)
        {
            Text = new string('a', 161) // 161 characters -- which exceeds limit
        };
        SetupPageContextWithAuthenticatedUser(publicPageModel, "TestUser");

        // here we manually trigger model validation (which simulates what ASP.NET core does when doing modelbinding)
        var validationContext = new ValidationContext(publicPageModel);
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        Validator.TryValidateObject(publicPageModel, validationContext, validationResults, true);

        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                publicPageModel.ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? "");
            }
        }

        var result = await publicPageModel.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.False(publicPageModel.ModelState.IsValid);
        Assert.True(publicPageModel.ModelState.ContainsKey("Text"));
        var validationError = publicPageModel.ModelState["Text"]?.Errors.FirstOrDefault();
        Assert.NotNull(validationError);
        Assert.Contains("160", validationError.ErrorMessage);
    }

    [Fact]
    public async Task PostingCheep_WhenMessageIsEmpty_ShowsRequiredFieldErrorAndDoesNotCreateCheep()
    {
        var service = CreateCheepServiceWithFreshDatabase();
        var publicPageModel = new PublicModel(service)
        {
            Text = string.Empty
        };
        SetupPageContextWithAuthenticatedUser(publicPageModel, "TestUser");

        // manual triggering the model validation
        var validationContext = new ValidationContext(publicPageModel);
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        Validator.TryValidateObject(publicPageModel, validationContext, validationResults, true);

        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                publicPageModel.ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? "");
            }
        }

        var result = await publicPageModel.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.False(publicPageModel.ModelState.IsValid);
        Assert.True(publicPageModel.ModelState.ContainsKey("Text"));
    }

    [Fact]
    public async Task PostingCheep_WhenMessageIsExactly160Characters_SuccessfullyCreatesCheep()
    {
        var service = CreateCheepServiceWithFreshDatabase();
        var publicPageModel = new PublicModel(service)
        {
            Text = new string('a', 160) // this is exactly 160 chars which is the allowed max
        };
        SetupPageContextWithAuthenticatedUser(publicPageModel, "TestUser");

        var result = await publicPageModel.OnPostAsync();

        Assert.IsType<RedirectToPageResult>(result);

        // verifying that the cheep was created with the exactly 160 charrs
        var allCheeps = service.GetCheeps(1, 32);
        Assert.Single(allCheeps);
        Assert.Equal(160, allCheeps[0].Message.Length);
    }

    [Fact]
    public async Task PostingCheepOnUserTimeline_WhenUserIsNotLoggedIn_ShowsErrorMessageAndDoesNotCreateCheep()
    {
        var service = CreateCheepServiceWithFreshDatabase();
        var userTimelinePageModel = new UserTimelineModel(service)
        {
            Text = "test cheep test cheep"
        };
        SetupPageContextWithUnauthenticatedUser(userTimelinePageModel);

        var result = await userTimelinePageModel.OnPostAsync("SomeAuthor");

        Assert.IsType<PageResult>(result);
        Assert.False(userTimelinePageModel.ModelState.IsValid);
        Assert.True(userTimelinePageModel.ModelState.ContainsKey(string.Empty));
        var errorMessage = userTimelinePageModel.ModelState[string.Empty]?.Errors.FirstOrDefault();
        Assert.NotNull(errorMessage);
        Assert.Equal("You must be logged in to post a cheep", errorMessage.ErrorMessage);
    }

    [Fact]
    public async Task PostingCheepOnUserTimeline_WhenUserIsAuthenticated_SuccessfullyCreatesCheepAndAppearsInUsersTimeline()
    {
        var service = CreateCheepServiceWithFreshDatabase();
        var userTimelinePageModel = new UserTimelineModel(service)
        {
            Text = "test -- valid test cheep from usertimeline"
        };
        SetupPageContextWithAuthenticatedUser(userTimelinePageModel, "TestUser");

        var result = await userTimelinePageModel.OnPostAsync("TestUser");

        Assert.IsType<RedirectToPageResult>(result);
        var redirectResult = result as RedirectToPageResult;
        Assert.Equal("/UserTimeline", redirectResult?.PageName);

        // verifying that the cheep got created and is appearing in the users timeline
        var userCheeps = service.GetCheepsFromAuthor("TestUser", 1, 32);
        Assert.Single(userCheeps);
        Assert.Equal("test -- valid test cheep from usertimeline", userCheeps[0].Message);
    }

    [Fact]
    public async Task PostingCheepOnPublicTimeline_WhenSuccessful_RedirectsBackToSamePageNumberToPreventResubmission()
    {
        var service = CreateCheepServiceWithFreshDatabase();
        var publicPageModel = new PublicModel(service)
        {
            Text = "Test cheep",
            CurrentPage = 3
        };
        SetupPageContextWithAuthenticatedUser(publicPageModel, "TestUser");

        var result = await publicPageModel.OnPostAsync();

        Assert.IsType<RedirectToPageResult>(result);
        var redirectResult = result as RedirectToPageResult;
        Assert.NotNull(redirectResult?.RouteValues);
        Assert.True(redirectResult.RouteValues.ContainsKey("page"));
        Assert.Equal(3, redirectResult.RouteValues["page"]);
    }
}
