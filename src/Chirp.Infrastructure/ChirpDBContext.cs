using Chirp.Core;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

/// <summary>
/// Entity Framework database context used for <i>Chirp!</i>
/// </summary>
public class ChirpDBContext : IdentityDbContext<Author>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        Cheeps = Set<Cheep>();
        Authors = Set<Author>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Cheep>()
        .HasOne(c => c.Author)
        .WithMany(a => a.Cheeps);

        builder.Entity<Cheep>()
            .HasMany(c => c.Likes)
            .WithMany(c => c.Likes)
            .UsingEntity(j => j.ToTable("CheepLikes"));

        builder.Entity<Author>()
            .HasMany(a => a.Following)
            .WithMany(a => a.Followers)
            .UsingEntity(j => j.ToTable("AuthorFollows"));
    }
}
