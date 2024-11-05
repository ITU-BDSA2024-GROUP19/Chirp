using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

public class Cheep {
    [Key]
    public int CheepId { get; set; }
    [Required]
    [StringLength(160)]
    public required string Text {get; set;}
    public required DateTime TimeStamp {get; set;}
    public required Author Author {get; set;}
}

public class Author : IdentityUser {
    public required ICollection<Cheep> Cheeps {get; set;}
}
