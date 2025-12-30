using Chirp.Core;
using Chirp.Infrastructure.Chirp.Repositories;
using Xunit;

namespace Chirp.Razor.Tests.SecurityTests;

public class SqlInjectionTests
{
    [Theory]
    [InlineData("'; DROP TABLE Cheeps; --")]
    [InlineData("' OR '1'='1")]
    [InlineData("'; DELETE FROM Authors; --")]
    [InlineData("1; UPDATE Authors SET Name='hacked' WHERE '1'='1")]
    [InlineData("Robert'); DROP TABLE Cheeps;--")]
    public async Task CheepText_WithSqlInjectionPayload_StoredAsLiteralText(string maliciousPayload)
    {
        var context = TestDbContextFactory.CreateContext($"SqlInjection_CheepText_{maliciousPayload.GetHashCode()}");
        var repository = new CheepRepository(context);

        var author = new Author
        {
            Name = "TestUser",
            Email = "test@test.com",
            Cheeps = new List<Cheep>()
        };
        context.Authors.Add(author);
        await context.SaveChangesAsync();

        await repository.CreateCheep("TestUser", maliciousPayload);

        var cheeps = repository.GetCheeps();
        Assert.Single(cheeps);
        Assert.Equal(maliciousPayload, cheeps[0].Text);
    }

    [Theory]
    [InlineData("' OR '1'='1")]
    [InlineData("' OR 1=1 --")]
    [InlineData("admin'--")]
    [InlineData("' UNION SELECT * FROM Authors --")]
    public async Task AuthorSearch_WithSqlInjectionPayload_ReturnsNoResults(string maliciousName)
    {
        var context = TestDbContextFactory.CreateContext($"SqlInjection_AuthorSearch_{maliciousName.GetHashCode()}");
        var authorRepository = new AuthorRepository(context);

        var realAuthor = new Author
        {
            Name = "RealUser",
            Email = "real@test.com",
            Cheeps = new List<Cheep>()
        };
        context.Authors.Add(realAuthor);
        await context.SaveChangesAsync();

        var result = await authorRepository.GetAuthorByName(maliciousName);

        Assert.Null(result);
        Assert.Single(context.Authors);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(int.MinValue)]
    public void Pagination_WithNegativeValues_HandlesGracefully(int maliciousPage)
    {
        var context = TestDbContextFactory.CreateContext($"SqlInjection_Pagination_{maliciousPage}");
        var repository = new CheepRepository(context);

        var author = new Author
        {
            Name = "PageUser",
            Email = "page@test.com",
            Cheeps = new List<Cheep>()
        };
        context.Authors.Add(author);
        context.Cheeps.Add(new Cheep
        {
            Author = author,
            Text = "Test cheep",
            TimeStamp = DateTime.UtcNow
        });
        context.SaveChanges();

        var exception = Record.Exception(() => repository.GetCheeps(maliciousPage, 10));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(999999999)]
    public void Pagination_WithExtremeValues_HandlesGracefully(int extremePage)
    {
        var context = TestDbContextFactory.CreateContext($"SqlInjection_ExtremePage_{extremePage}");
        var repository = new CheepRepository(context);

        var author = new Author
        {
            Name = "ExtremeUser",
            Email = "extreme@test.com",
            Cheeps = new List<Cheep>()
        };
        context.Authors.Add(author);
        context.Cheeps.Add(new Cheep
        {
            Author = author,
            Text = "Test cheep",
            TimeStamp = DateTime.UtcNow
        });
        context.SaveChanges();

        var exception = Record.Exception(() => repository.GetCheeps(extremePage, 10));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData("'; DROP TABLE Authors; --")]
    [InlineData("' OR '1'='1' --")]
    [InlineData("victim'; UPDATE Authors SET followings='attacker' WHERE Name='victim'; --")]
    public async Task FollowingList_WithInjectionPayload_TreatedAsLiteral(string maliciousTarget)
    {
        var context = TestDbContextFactory.CreateContext($"SqlInjection_Following_{maliciousTarget.GetHashCode()}");
        var repository = new CheepRepository(context);

        var author = new Author
        {
            Name = "FollowUser",
            Email = "follow@test.com",
            Cheeps = new List<Cheep>(),
            followings = new List<string>()
        };
        context.Authors.Add(author);
        await context.SaveChangesAsync();

        var result = await repository.AddToFollowing("FollowUser", maliciousTarget);

        Assert.False(result);
        var updatedAuthor = context.Authors.First(a => a.Name == "FollowUser");
        Assert.Empty(updatedAuthor.followings);
    }
}
