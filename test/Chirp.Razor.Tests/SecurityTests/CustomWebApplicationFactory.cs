using Chirp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Chirp.Razor.Tests.SecurityTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // removing the already current DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ChirpDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // then adding in memory db for testing
            services.AddDbContext<ChirpDbContext>(options =>
            {
                options.UseInMemoryDatabase("CsrfTestDb_" + Guid.NewGuid());
            });
        });

        builder.UseEnvironment("Development");
    }
}
