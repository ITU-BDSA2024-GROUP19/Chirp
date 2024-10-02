using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class Cheep {
    [Key]
    public required string Text {get; set;}
    public required DateTime TimeStamp {get; set;}
    public required Author Author {get; set;}
}

public class Author {
    [Key]
    public required string Name {get; set;}
    public required string Email {get; set;}
    public required ICollection<Cheep> Cheeps {get; set;}
}

public class ChirpDBContext : DbContext  {
    public DbSet<Cheep> Cheeps {get; set;}
    public DbSet<Author> Authors {get; set;}
    public ChirpDBContext (DbContextOptions<ChirpDBContext> options) : base(options){
    }
}
