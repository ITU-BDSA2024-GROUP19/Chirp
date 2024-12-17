using Chirp.Infrastructure.Cheeps;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Actions;

/// <summary>
/// <para>Form post page for handling likes for Cheeps.</para>
/// <para>
/// - Information on how OnPost handlers work:
/// https://www.aspsnippets.com/Articles/3165/Using-the-OnPost-handler-method-in-ASPNet-Core-Razor-Pages/#google_vignette
/// </para>
/// </summary>
public class AuthorLikesModel : PageModel
{
    private readonly ICheepService _cheepService;

    public AuthorLikesModel(ICheepService cheepService)
    {
        _cheepService = cheepService;
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
    /// Post to Chirp! that the calling user likes a specific Cheep.
    /// </summary>
    /// <param name="authorName">Cheep to like.</param>
    /// <param name="returnUrl"></param>
    /// <returns>Redirect to returnUrl.</returns>
    public IActionResult OnPostLike(int cheepId, string? returnUrl = null)
    {
        Console.WriteLine("AuthorLikes OnPostLike(...) called");
        Console.WriteLine(User.Identity?.Name! + " wants to like cheep: " + cheepId);
        returnUrl ??= Url.Content("~/");

        _cheepService.LikeCheep(cheepId, User.Identity?.Name!);
        return LocalRedirect(returnUrl);
    }

    /// <summary>
    /// Post to Chirp! that the calling user no longer likes a specific Cheep.
    /// </summary>
    /// <param name="authorName">Cheep to remove like from.</param>
    /// <param name="returnUrl"></param>
    /// <returns>Redirect to returnUrl.</returns>
    public IActionResult OnPostRemoveLike(int cheepId, string? returnUrl = null)
    {
        Console.WriteLine("AuthorLikes OnPostRemoveLike(...) called");
        Console.WriteLine(User.Identity?.Name! + " wants to remove like cheep: " + cheepId);
        returnUrl ??= Url.Content("~/");

        _cheepService.RemoveLikeCheep(cheepId, User.Identity?.Name!);
        return LocalRedirect(returnUrl);
    }
}