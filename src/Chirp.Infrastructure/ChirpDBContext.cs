using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpDBContext : IdentityDbContext<Author>  {
    public DbSet<Cheep> Cheeps {get; set;}
    public DbSet<Author> Authors {get; set;}
    public ChirpDBContext (DbContextOptions<ChirpDBContext> options) : base(options){
    }
}
