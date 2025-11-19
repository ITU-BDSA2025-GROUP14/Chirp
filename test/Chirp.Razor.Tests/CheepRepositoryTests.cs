using Chirp.Core;
using Chirp.Infrastructure.Chirp.Repositories;

using Microsoft.EntityFrameworkCore;

using Xunit;

public class CheepRepositoryTests
{
    [Fact]
    public async Task CreateCheep_AddsNewCheep_Successfully()
    {
        // for arranging
        var context = TestDbContextFactory.CreateContext("CreateCheep_Test");
        var repository = new CheepRepository(context);

        // we add an already existingn author to begin with
        var author = new Author
        {
            Name = "TestUser",
            Email = "test@test.com",
            Cheeps = new List<Cheep>()
        };
        context.Authors.Add(author);
        await context.SaveChangesAsync();

        await repository.CreateCheep("TestUser", "test cheep");

        // Assert
        var cheeps = context.Cheeps.Include(c => c.Author).ToList();
        Assert.Single(cheeps);
        Assert.Equal("test cheep", cheeps[0].Text);
        Assert.Equal("TestUser", cheeps[0].Author.Name);
    }

    [Fact]
    public async Task CreateCheep_CreatesNewAuthor_WhenAuthorDoesNotExist()
    {
        var context = TestDbContextFactory.CreateContext("CreateCheep_NewAuthor_Test");
        var repository = new CheepRepository(context);

        await repository.CreateCheep("NewUser", "test from new user");

        var authors = context.Authors.ToList();
        Assert.Single(authors);
        Assert.Equal("NewUser", authors[0].Name);

        var cheeps = context.Cheeps.Include(c => c.Author).ToList();
        Assert.Single(cheeps);
        Assert.Equal("test from new user", cheeps[0].Text);
        Assert.Equal("NewUser", cheeps[0].Author.Name);
    }

    [Fact]
    public async Task CreateCheep_SetsTimestamp_ToCurrentTime()
    {
        var context = TestDbContextFactory.CreateContext("CreateCheep_Timestamp_Test");
        var repository = new CheepRepository(context);
        var beforeTime = DateTime.UtcNow.AddSeconds(-1);

        await repository.CreateCheep("TimeUser", "testing timestamp");
        var afterTime = DateTime.UtcNow.AddSeconds(1);

        var cheep = context.Cheeps.First();
        Assert.True(cheep.TimeStamp >= beforeTime);
        Assert.True(cheep.TimeStamp <= afterTime);
    }

    [Fact]
    public async Task CreateCheep_AppearsInGetCheeps()
    {
        var context = TestDbContextFactory.CreateContext("CreateCheep_GetCheeps_Test");
        var repository = new CheepRepository(context);

        await repository.CreateCheep("GetUser", "testing retrieval message");

        var cheeps = repository.GetCheeps();
        Assert.Single(cheeps);
        Assert.Equal("testing retrieval message", cheeps[0].Text);
        Assert.Equal("GetUser", cheeps[0].Author.Name);
    }

    [Fact]
    public async Task CreateCheep_AppearsInAuthorTimeline()
    {
        var context = TestDbContextFactory.CreateContext("CreateCheep_AuthorTimeline_Test");
        var repository = new CheepRepository(context);

        await repository.CreateCheep("AuthorUser", "test cheep for author");

        var cheeps = repository.GetCheepsFromAuthor("AuthorUser");
        Assert.Single(cheeps);
        Assert.Equal("test cheep for author", cheeps[0].Text);
    }
}
