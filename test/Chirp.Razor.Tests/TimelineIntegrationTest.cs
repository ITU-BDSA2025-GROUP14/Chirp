using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Chirp.Services;
using Chirp.Web.Pages;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using Xunit;

namespace Chirp.Razor.Tests;

public class TimelineIntegrationTest
{
    private readonly UserTimelineModel _pageModel;
    private readonly ChirpDbContext _context;
    private readonly CheepService _service;

    public TimelineIntegrationTest()
    {
        _context = TestDbContextFactory.CreateContext(Guid.NewGuid().ToString());


        // seed some data
        var author = new Author
        {
            Name = "Alice",
            Email = "alice@example.com"
        };
        _context.Authors.Add(author);
        _context.Cheeps.AddRange(
            new Cheep { Author = author, Text = "First", TimeStamp = DateTime.UtcNow },
            new Cheep { Author = author, Text = "Second", TimeStamp = DateTime.UtcNow.AddMinutes(-1) }
        );
        _context.SaveChanges();

        var repo = new CheepRepository(_context);
        var authorRepo = new AuthorRepository(_context);
        var likeRepo = new LikeRepository(_context);
        var svc = new CheepService(repo, likeRepo);
        _pageModel = new UserTimelineModel(svc, authorRepo);
        
        _pageModel.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };
    }

    [Fact]
    public async Task OnGet_ShouldLoadUserCheeps()
    {
        var result = await _pageModel.OnGetAsync("Alice");

        Assert.IsType<PageResult>(result);
        Assert.Equal("Alice", _pageModel.Author);
        Assert.NotEmpty(_pageModel.Cheeps);
        Assert.Equal(2, _pageModel.PageCount);
    }
}