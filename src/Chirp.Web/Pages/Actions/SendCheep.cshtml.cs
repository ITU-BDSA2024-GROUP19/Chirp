using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Cheeps;
using Microsoft.AspNetCore.Identity;
using Chirp.Core;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages.Actions;

public class SendCheepModel : PageModel
{
    public class InputModel
    {
        [Required]
        [StringLength(160, MinimumLength = 1, ErrorMessage = "Maximum length is {1}")]
        [Display(Name = "Message Text")]
        public string? Message { get; set; }
    }

    private readonly ICheepService _cheepService;
    private readonly SignInManager<Author> _signInManager;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SendCheepModel(
        ICheepService cheepService, 
        SignInManager<Author> signInManager)
    {
        _cheepService = cheepService;
        _signInManager = signInManager;
    }

    public IActionResult OnGet()
    {
        return new StatusCodeResult(405); // Method not allowed
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        Console.WriteLine("SendCheep OnPostAsync(...) called");
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

        _cheepService.AddCheep(author, Input.Message ?? throw new NullReferenceException());
        return LocalRedirect(returnUrl);
    }
}
