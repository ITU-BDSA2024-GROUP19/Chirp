using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

public class Cheep {
    [Key]
    public required int CheepId { get; set; }
    public required int AuthorId { get; set; }
    [Required]
    [StringLength(160)]
    public required string Text {get; set;}
    public required DateTime TimeStamp {get; set;}
    public required Author Author {get; set;}
}

public class Author {
    [Key]
    public required int AuthorId { get; set; }
    public required string Name {get; set;}
    public required string Email {get; set;}
    public required ICollection<Cheep> Cheeps {get; set;}
}
