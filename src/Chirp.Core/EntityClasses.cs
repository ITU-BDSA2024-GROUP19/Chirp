using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

public class Cheep 
{
    [Key]
    public int CheepId { get; set; }
    [Required]
    [StringLength(160)]
    public required string Text { get; set; }
    [Required]
    public required DateTime TimeStamp { get; set; }
    [Required]
    public required Author Author { get; set; }
}

public class Author : IdentityUser 
{
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public ICollection<Author> Following { get; set; } = new List<Author>();
    public ICollection<Author> Followers { get; set ;} = new List<Author>();

    public void AddSampleCheep(string text, DateTime date) 
    {
        Cheeps.Add(new Cheep() { Author = this, Text = text, TimeStamp = date });
    }
}
