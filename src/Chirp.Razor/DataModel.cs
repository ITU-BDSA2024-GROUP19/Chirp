using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class Cheep {
    [Key]
    public string Text {get; set;}
    public DateTime TimeStamp {get; set;}
    public Author Author {get; set;}
}

public class Author {
    [Key]
    public string Name {get; set;}
    public string Email {get; set;}
    public ICollection<Cheep> Cheeps {get; set;}
}

public class ChirpDBContext : DbContext  {
    public DbSet<Cheep> Cheeps {get; set;}
    public DbSet<Author> Authors {get; set;}
    public ChirpDBContext (DbContextOptions<ChirpDBContext> options) : base(options){
    }
}
