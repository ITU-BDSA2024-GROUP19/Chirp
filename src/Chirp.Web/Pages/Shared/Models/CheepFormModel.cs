using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages.Shared.Models;

public class CheepFormModel
{
    [Required]
    [StringLength(160, ErrorMessage = "Maximum length is {1}")]
    [Display(Name = "Message Text")]
    public string ?Message { get; set; }
}
