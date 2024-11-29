using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpDBContext : IdentityDbContext<Author>  
{
    public required DbSet<Cheep> Cheeps { get; set; }
    public required DbSet<Author> Authors { get; set; }
    public ChirpDBContext (DbContextOptions<ChirpDBContext> options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder builder) 
    {
        base.OnModelCreating(builder);
        builder.Entity<Author>()
            .HasMany(a => a.Following)
            .WithMany(a => a.Followers)
            .UsingEntity(j => j.ToTable("AuthorFollows"));
    }
}
