using Chirp.Razor.Models;
using Chirp.Web.Pages;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Chirp.Razor.Tests;

public class TimelineIntegrationTest
{
    private readonly UserTimelineModel _pageModel;
    private readonly ChirpDBContext _context;
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
        var svc = new CheepService(repo);
        _pageModel = new UserTimelineModel(svc);
    }

    [Fact]
    public void OnGet_ShouldLoadUserCheeps()
    {
        var result = _pageModel.OnGet("Alice");

        Assert.IsType<PageResult>(result);
        Assert.Equal("Alice", _pageModel.Author);
        Assert.NotEmpty(_pageModel.Cheeps);
        Assert.Equal(2, _pageModel.PageCount);
    }
}