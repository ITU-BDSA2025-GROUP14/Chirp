using Chirp.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Chirp.Razor;

public static class TestDbContextFactory
{
    public static ChirpDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ChirpDbContext(options);
    }
}