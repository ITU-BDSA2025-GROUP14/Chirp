using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Chirp.Services;

using Xunit;
using Chirp.Web.Pages;
using Chirp.Razor;

using Microsoft.AspNetCore.Mvc.RazorPages;

public class PublicPageTests
{
    private readonly PublicModel _pageModel;
    private readonly ChirpDbContext _context;
    private readonly CheepService _service;

    public PublicPageTests()
    {
        _context = TestDbContextFactory.CreateContext(Guid.NewGuid().ToString());

        var author = new Author
        {
            Name = "Alice",
            Email = "alice@example.com"
        };

        _context.Authors.Add(author);
        _context.Cheeps.Add(new Cheep
        {
            Author = author,
            Text = "Hello world",
            TimeStamp = DateTime.UtcNow
        });
        _context.SaveChanges();

        var repo = new CheepRepository(_context);
        var authorRepo = new AuthorRepository(_context);
        _service = new CheepService(repo);
        _pageModel = new PublicModel(_service, authorRepo);
    }

    [Fact]
    public void OnGet_ShouldLoadCheepsAndReturnPage()
    {
        var result = _pageModel.OnGetAsync(1);

        Assert.IsType<PageResult>(result);
        Assert.NotEmpty(_pageModel.Cheeps);
        Assert.Equal(1, _pageModel.PageCount);
        Assert.False(_pageModel.ShowPrevious);
    }
    
    [Fact]
    public void Public_OnGet_PageLessThanOne_ClampsTo1()
    {
        var ctx = TestDbContextFactory.CreateContext(Guid.NewGuid().ToString());
        var author = new Author { Name = "Alice", Email = "alice@example.com" };
        ctx.Authors.Add(author);

        for (int i = 0; i < 60; i++)
            ctx.Cheeps.Add(
                new Cheep
                {
                    Author = author,
                    Text = $"#{i}",
                    TimeStamp = DateTime.UtcNow.AddSeconds(-i)
                });
        ctx.SaveChanges();

        var repo = new CheepRepository(ctx);
        var authorRepo = new AuthorRepository(ctx);
        var svc = new CheepService(repo);
        var page = new PublicModel(svc, authorRepo);

        var result = page.OnGetAsync(0); // clamp
        Assert.IsType<PageResult>(result);
        Assert.Equal(1, page.CurrentPage);
        Assert.Equal(60, page.PageCount);
        Assert.Equal(32, page.Cheeps.Count);
        Assert.True(page.ShowNext);
        Assert.False(page.ShowPrevious);
    }

    [Fact]
    public void Timeline_OnGet_UnknownAuthor_ReturnsEmpty()
    {
        var ctx = TestDbContextFactory.CreateContext(Guid.NewGuid().ToString());
        var repo = new CheepRepository(ctx);
        var authorRepo = new AuthorRepository(ctx);
        var svc = new CheepService(repo);
        var page = new UserTimelineModel(svc, authorRepo);

        var result = page.OnGetAsync("nobody");
        Assert.IsType<PageResult>(result);
        Assert.Equal("nobody", page.Author);
        Assert.Empty(page.Cheeps);
        Assert.Equal(0, page.PageCount);
        Assert.False(page.ShowNext);
        Assert.False(page.ShowPrevious);
    }

    [Fact]
    public void Timeline_Order_IsStable_WhenTimestampsEqual()
    {
        var ctx = TestDbContextFactory.CreateContext(Guid.NewGuid().ToString());
        var author = new Author { Name = "Alice", Email = "alice@example.com" };
        ctx.Authors.Add(author);
        var t = DateTime.UtcNow;

        ctx.Cheeps.AddRange(
            new Cheep { Author = author, Text = "B", TimeStamp = t },
            new Cheep { Author = author, Text = "A", TimeStamp = t }
        );
        ctx.SaveChanges();

        var repo = new CheepRepository(ctx);
        var authorRepo = new AuthorRepository(ctx);
        var svc = new CheepService(repo);
        var page = new UserTimelineModel(svc, authorRepo) { page = 1 };
        page.OnGetAsync("Alice");

        // Expect most recent CheepId first
        Assert.Equal(new[] { "A", "B" }, page.Cheeps.Select(c => c.Message).ToArray());
    }

}