using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

/// <summary>
/// A Cheep is a message on Chirp. 
/// </summary>
public class Cheep 
{
    /// <summary>Primary key for cheeps. Generated always.</summary>
    [Key]
    public int CheepId { get; set; }
    [Required]
    [StringLength(160)]
    public required string Text { get; set; }
    [Required]
    public required DateTime TimeStamp { get; set; }
    [Required]
    public required Author Author { get; set; }
    public ICollection<Author> Likes { get; set; } = new List<Author>();
}

/// <summary>
/// An Author is a user on Chirp. 
/// It extends the default IdentityUser type with fields specific to the Chirp application. 
/// </summary>
public class Author : IdentityUser 
{
    [StringLength(500)]
    public string ProfilePicture { get; set; } = "/images/icon1.png";
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public ICollection<Cheep> Likes { get; set; } = new List<Cheep>();
    public ICollection<Author> Following { get; set; } = new List<Author>();
    public ICollection<Author> Followers { get; set ;} = new List<Author>();

    /// <summary>
    /// <para>Adds a new Cheep to this author with a given timestamp.</para>
    /// <para>This method is provided solely to add sample Cheeps to sample accounts.</para>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="timeStamp"></param>
    public void AddSampleCheep(string text, DateTime timeStamp) 
    {
        Cheeps.Add(new Cheep() { Author = this, Text = text, TimeStamp = timeStamp});
    }
}
