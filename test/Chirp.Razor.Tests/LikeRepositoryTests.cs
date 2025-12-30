using Chirp.Core;
using Chirp.Infrastructure.Chirp.Repositories;

using Microsoft.EntityFrameworkCore;

using Xunit;

public class LikeRepositoryTests
{
    private async Task<(Author author, Cheep cheep)> CreateTestAuthorAndCheep(ChirpDbContext context, string authorName = "TestUser")
    {
        var author = new Author
        {
            Name = authorName,
            Email = $"{authorName.ToLower()}@test.com",
            Cheeps = new List<Cheep>(),
            Likes = new List<Like>()
        };
        context.Authors.Add(author);
        await context.SaveChangesAsync();

        var cheep = new Cheep
        {
            AuthorId = author.AuthorId,
            Author = author,
            Text = "Test cheep content",
            TimeStamp = DateTime.UtcNow,
            Likes = new List<Like>()
        };
        context.Cheeps.Add(cheep);
        await context.SaveChangesAsync();

        return (author, cheep);
    }

    [Fact]
    public async Task AddLike_CreatesNewLike_WhenNotAlreadyLiked()
    {
        // arranging
        var context = TestDbContextFactory.CreateContext("AddLike_CreatesNewLike_Test");
        var repository = new LikeRepository(context);
        var (author, cheep) = await CreateTestAuthorAndCheep(context);

        // acting
        var result = await repository.AddLike(cheep.CheepId, author.Name);

        // aaaaaaand asserting
        Assert.True(result);
        var like = await context.Likes.FirstOrDefaultAsync(l => l.CheepId == cheep.CheepId && l.AuthorId == author.AuthorId);
        Assert.NotNull(like);
        Assert.Equal(cheep.CheepId, like.CheepId);
        Assert.Equal(author.AuthorId, like.AuthorId);
    }

    [Fact]
    public async Task AddLike_ReturnsFalse_WhenAlreadyLiked()
    {
        // arranging
        var context = TestDbContextFactory.CreateContext("AddLike_ReturnsFalse_Test");
        var repository = new LikeRepository(context);
        var (author, cheep) = await CreateTestAuthorAndCheep(context);

        // we then add the like first
        var like = new Like
        {
            CheepId = cheep.CheepId,
            AuthorId = author.AuthorId,
            TimeStamp = DateTime.UtcNow
        };
        context.Likes.Add(like);
        await context.SaveChangesAsync();

        // acting
        var result = await repository.AddLike(cheep.CheepId, author.Name);

        // asserting
        Assert.False(result);
        var likeCount = await context.Likes.CountAsync(l => l.CheepId == cheep.CheepId && l.AuthorId == author.AuthorId);
        Assert.Equal(1, likeCount); // it should still onlyl be 1 like
    }

    [Fact]
    public async Task RemoveLike_DeletesLike_WhenExists()
    {
        // arranging
        var context = TestDbContextFactory.CreateContext("RemoveLike_DeletesLike_Test");
        var repository = new LikeRepository(context);
        var (author, cheep) = await CreateTestAuthorAndCheep(context);

        // we add a a like first
        var like = new Like
        {
            CheepId = cheep.CheepId,
            AuthorId = author.AuthorId,
            TimeStamp = DateTime.UtcNow
        };
        context.Likes.Add(like);
        await context.SaveChangesAsync();

        // acting
        var result = await repository.RemoveLike(cheep.CheepId, author.Name);

        // asserting
        Assert.True(result);
        var likeExists = await context.Likes.AnyAsync(l => l.CheepId == cheep.CheepId && l.AuthorId == author.AuthorId);
        Assert.False(likeExists);
    }

    [Fact]
    public async Task RemoveLike_ReturnsFalse_WhenLikeDoesNotExist()
    {
        // arranging
        var context = TestDbContextFactory.CreateContext("RemoveLike_ReturnsFalse_Test");
        var repository = new LikeRepository(context);
        var (author, cheep) = await CreateTestAuthorAndCheep(context);

        // acting
        var result = await repository.RemoveLike(cheep.CheepId, author.Name);

        // asserting
        Assert.False(result);
    }

    [Fact]
    public async Task HasUserLiked_ReturnsTrue_WhenLiked()
    {
        // arranging
        var context = TestDbContextFactory.CreateContext("HasUserLiked_ReturnsTrue_Test");
        var repository = new LikeRepository(context);
        var (author, cheep) = await CreateTestAuthorAndCheep(context);

        // we add a like
        var like = new Like
        {
            CheepId = cheep.CheepId,
            AuthorId = author.AuthorId,
            TimeStamp = DateTime.UtcNow
        };
        context.Likes.Add(like);
        await context.SaveChangesAsync();

        // acting
        var result = await repository.HasUserLiked(cheep.CheepId, author.Name);

        // asserting
        Assert.True(result);
    }

    [Fact]
    public async Task HasUserLiked_ReturnsFalse_WhenNotLiked()
    {
        // arranging
        var context = TestDbContextFactory.CreateContext("HasUserLiked_ReturnsFalse_Test");
        var repository = new LikeRepository(context);
        var (author, cheep) = await CreateTestAuthorAndCheep(context);

        // acting
        var result = await repository.HasUserLiked(cheep.CheepId, author.Name);

        // asserting
        Assert.False(result);
    }

    [Fact]
    public async Task GetLikeCount_ReturnsCorrectCount()
    {
        // arranging
        var context = TestDbContextFactory.CreateContext("GetLikeCount_Test");
        var repository = new LikeRepository(context);

        // we create a cheep author
        var cheepAuthor = new Author
        {
            Name = "CheepAuthor",
            Email = "cheepauthor@test.com",
            Cheeps = new List<Cheep>(),
            Likes = new List<Like>()
        };
        context.Authors.Add(cheepAuthor);
        await context.SaveChangesAsync();

        var cheep = new Cheep
        {
            AuthorId = cheepAuthor.AuthorId,
            Author = cheepAuthor,
            Text = "Test cheep",
            TimeStamp = DateTime.UtcNow,
            Likes = new List<Like>()
        };
        context.Cheeps.Add(cheep);
        await context.SaveChangesAsync();

        // we create several authors that like (likinguser)
        for (int i = 1; i <= 5; i++)
        {
            var likingAuthor = new Author
            {
                Name = $"LikingUser{i}",
                Email = $"likinguser{i}@test.com",
                Cheeps = new List<Cheep>(),
                Likes = new List<Like>()
            };
            context.Authors.Add(likingAuthor);
            await context.SaveChangesAsync();

            var like = new Like
            {
                CheepId = cheep.CheepId,
                AuthorId = likingAuthor.AuthorId,
                TimeStamp = DateTime.UtcNow
            };
            context.Likes.Add(like);
        }
        await context.SaveChangesAsync();

        // acting
        var count = await repository.GetLikeCount(cheep.CheepId);

        // asserting
        Assert.Equal(5, count);
    }

    [Fact]
    public async Task GetLikeCounts_ReturnsBatchCounts()
    {
        // arranging
        var context = TestDbContextFactory.CreateContext("GetLikeCounts_BatchTest");
        var repository = new LikeRepository(context);

        // creating test author
        var author = new Author
        {
            Name = "TestAuthor",
            Email = "testauthor@test.com",
            Cheeps = new List<Cheep>(),
            Likes = new List<Like>()
        };
        context.Authors.Add(author);
        await context.SaveChangesAsync();

        // creating several cheeps with different amounts of likess
        var cheep1 = new Cheep
        {
            AuthorId = author.AuthorId,
            Author = author,
            Text = "Cheep 1",
            TimeStamp = DateTime.UtcNow,
            Likes = new List<Like>()
        };
        var cheep2 = new Cheep
        {
            AuthorId = author.AuthorId,
            Author = author,
            Text = "Cheep 2",
            TimeStamp = DateTime.UtcNow,
            Likes = new List<Like>()
        };
        var cheep3 = new Cheep
        {
            AuthorId = author.AuthorId,
            Author = author,
            Text = "Cheep 3",
            TimeStamp = DateTime.UtcNow,
            Likes = new List<Like>()
        };
        context.Cheeps.AddRange(cheep1, cheep2, cheep3);
        await context.SaveChangesAsync();

        // creating likingauthors and adding likes
        // cheep 1: 2 likes , cheep 2: 3 likes , cheep 3: 0 likes
        for (int i = 1; i <= 5; i++)
        {
            var likingAuthor = new Author
            {
                Name = $"Liker{i}",
                Email = $"liker{i}@test.com",
                Cheeps = new List<Cheep>(),
                Likes = new List<Like>()
            };
            context.Authors.Add(likingAuthor);
            await context.SaveChangesAsync();

            if (i <= 2)
            {
                context.Likes.Add(new Like { CheepId = cheep1.CheepId, AuthorId = likingAuthor.AuthorId, TimeStamp = DateTime.UtcNow });
            }
            if (i <= 3)
            {
                context.Likes.Add(new Like { CheepId = cheep2.CheepId, AuthorId = likingAuthor.AuthorId, TimeStamp = DateTime.UtcNow });
            }
        }
        await context.SaveChangesAsync();

        // actting
        var cheepIds = new List<int> { cheep1.CheepId, cheep2.CheepId, cheep3.CheepId };
        var counts = await repository.GetLikeCounts(cheepIds);

        // asserting
        Assert.Equal(3, counts.Count);
        Assert.Equal(2, counts[cheep1.CheepId]);
        Assert.Equal(3, counts[cheep2.CheepId]);
        Assert.Equal(0, counts[cheep3.CheepId]);
    }

    [Fact]
    public async Task GetLikedCheepIds_ReturnsCorrectIds()
    {
        // arranging
        var context = TestDbContextFactory.CreateContext("GetLikedCheepIds_Test");
        var repository = new LikeRepository(context);

        // creating a cheepowner author
        var cheepAuthor = new Author
        {
            Name = "CheepOwner",
            Email = "cheepowner@test.com",
            Cheeps = new List<Cheep>(),
            Likes = new List<Like>()
        };
        context.Authors.Add(cheepAuthor);
        await context.SaveChangesAsync();

        // creating a likingauthor (likinguser)
        var likingAuthor = new Author
        {
            Name = "LikingUser",
            Email = "likinguser@test.com",
            Cheeps = new List<Cheep>(),
            Likes = new List<Like>()
        };
        context.Authors.Add(likingAuthor);
        await context.SaveChangesAsync();

        // creating several cheeps
        var cheep1 = new Cheep
        {
            AuthorId = cheepAuthor.AuthorId,
            Author = cheepAuthor,
            Text = "Cheep 1",
            TimeStamp = DateTime.UtcNow,
            Likes = new List<Like>()
        };
        var cheep2 = new Cheep
        {
            AuthorId = cheepAuthor.AuthorId,
            Author = cheepAuthor,
            Text = "Cheep 2",
            TimeStamp = DateTime.UtcNow,
            Likes = new List<Like>()
        };
        var cheep3 = new Cheep
        {
            AuthorId = cheepAuthor.AuthorId,
            Author = cheepAuthor,
            Text = "Cheep 3",
            TimeStamp = DateTime.UtcNow,
            Likes = new List<Like>()
        };
        context.Cheeps.AddRange(cheep1, cheep2, cheep3);
        await context.SaveChangesAsync();

        // the user likes cheep1 and cheep3 (and NOT cheep2)
        context.Likes.Add(new Like { CheepId = cheep1.CheepId, AuthorId = likingAuthor.AuthorId, TimeStamp = DateTime.UtcNow });
        context.Likes.Add(new Like { CheepId = cheep3.CheepId, AuthorId = likingAuthor.AuthorId, TimeStamp = DateTime.UtcNow });
        await context.SaveChangesAsync();

        // acting
        var cheepIds = new List<int> { cheep1.CheepId, cheep2.CheepId, cheep3.CheepId };
        var likedIds = await repository.GetLikedCheepIds(likingAuthor.Name, cheepIds);

        // asserting
        Assert.Equal(2, likedIds.Count);
        Assert.Contains(cheep1.CheepId, likedIds);
        Assert.Contains(cheep3.CheepId, likedIds);
        Assert.DoesNotContain(cheep2.CheepId, likedIds);
    }

    [Fact]
    public async Task GetLikedCheepIds_ReturnsEmptySet_WhenNoLikes()
    {
        // arranging
        var context = TestDbContextFactory.CreateContext("GetLikedCheepIds_Empty_Test");
        var repository = new LikeRepository(context);
        var (author, cheep) = await CreateTestAuthorAndCheep(context);

        // acting
        var cheepIds = new List<int> { cheep.CheepId };
        var likedIds = await repository.GetLikedCheepIds(author.Name, cheepIds);

        // assetrting :D
        Assert.Empty(likedIds);
    }
}
