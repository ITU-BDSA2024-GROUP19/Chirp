using System.ComponentModel.DataAnnotations;

using Chirp.Core;
using Chirp.Infrastructure.Cheeps;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Actions;

/// <summary>
/// <para>
/// Form post page for posting new Cheeps.
/// </para><para>
/// Works with the "_SendCheepForm" partial.
/// Pages embedding this form must contain SendCheepModel.InputModel as a bind property
/// and pass it to the form partial.
/// </para><para>
/// - For more information on form tag helpers and bind properties:
/// https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-9.0
/// </para>
/// </summary>
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

    /// <summary>
    /// HTTP GET is not supported by this page.
    /// </summary>
    /// <returns>Always: 405 - Method Not Allowed</returns>
    public IActionResult OnGet()
    {
        return new StatusCodeResult(405); // Method not allowed
    }

    /// <summary>
    /// Post a new Cheep to Chirp!
    /// Input to this handler is taken from model bind properties.
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <returns>Redirect to returnUrl.</returns>
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
