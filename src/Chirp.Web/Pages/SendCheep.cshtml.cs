using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Cheeps;
using Chirp.Infrastructure.Authors;
using Chirp.Web.Pages.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Chirp.Core;
using Chirp.Web.Pages.Models;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages;

public class SendCheepModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly SignInManager<Author> _signInManager;

    [Required]
    [StringLength(160, ErrorMessage = "Maximum length is {1}")]
    [Display(Name = "Message Text")]
    public string ?Message { get; set; }

    public SendCheepModel(
        ICheepService cheepService, 
        SignInManager<Author> signInManager)
    {
        _cheepService = cheepService;
        _signInManager = signInManager;
    }

    public IActionResult OnGet()
        {
            return NotFound();
        }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return LocalRedirect(returnUrl);
        }

        Author? author = await _signInManager.UserManager.GetUserAsync(User);
        if (author == null)
        {
            return Forbid("You are not logged in!!!");
        }

        _cheepService.AddCheep(author, Message ?? throw new NullReferenceException());
        return LocalRedirect(returnUrl);
    }
}
