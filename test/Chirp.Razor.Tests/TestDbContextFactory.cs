using Microsoft.EntityFrameworkCore;
using Chirp.Razor;

public static class TestDbContextFactory
{
    public static ChirpDBContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ChirpDBContext(options);
    }
}