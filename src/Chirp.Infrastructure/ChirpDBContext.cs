using Chirp.Core;
using Chirp.Infrastructure.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Like> Likes { get; set; }

    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(l => l.LikeId);

            entity.HasOne(l => l.Cheep)
                .WithMany(c => c.Likes)
                .HasForeignKey(l => l.CheepId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(l => l.Author)
                .WithMany(a => a.Likes)
                .HasForeignKey(l => l.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(l => new { l.CheepId, l.AuthorId }).IsUnique();
        });
    }
}