using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure;

public class ChirpDBContext : DbContext  {
    public DbSet<Cheep> Cheeps {get; set;}
    public DbSet<Author> Authors {get; set;}
    public ChirpDBContext (DbContextOptions<ChirpDBContext> options) : base(options){
    }
}
