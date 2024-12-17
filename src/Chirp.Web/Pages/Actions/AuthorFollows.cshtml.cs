using Chirp.Infrastructure.Authors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Actions;

/// <summary>
/// Form post page for following and un-following other Authors.
/// 
/// - Information on how OnPost handlers work:
/// https://www.aspsnippets.com/Articles/3165/Using-the-OnPost-handler-method-in-ASPNet-Core-Razor-Pages/#google_vignette
/// </summary>
public class AuthorFollowsModel : PageModel
{
    private readonly IAuthorService _authorService;

    public AuthorFollowsModel(IAuthorService authorService)
    {
        _authorService = authorService;
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
    /// Updates Chirp! so that the calling user now follows a specified user.
    /// </summary>
    /// <param name="authorName">Author to follow.</param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    public IActionResult OnPostFollow(string authorName, string? returnUrl = null)
    {
        Console.WriteLine("AuthorFollows OnPostFollow(...) called");
        Console.WriteLine(User.Identity?.Name! + " wants to follow author: " + authorName);
        returnUrl ??= Url.Content("~/");

        _authorService.FollowAuthor(User.Identity?.Name!, authorName);
        return LocalRedirect(returnUrl);
    }

    /// <summary>
    /// Updates Chirp! so that the calling user no longer follows a specified user.
    /// </summary>
    /// <param name="authorName">Author to unfollow.</param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    public IActionResult OnPostUnfollow(string authorName, string? returnUrl = null)
    {
        Console.WriteLine("AuthorFollows OnPostUnfollow(...) called");
        Console.WriteLine(User.Identity?.Name! + " wants to UNfollow author: " + authorName);
        returnUrl ??= Url.Content("~/");

        _authorService.UnfollowAuthor(User.Identity?.Name!, authorName);
        return LocalRedirect(returnUrl);
    }
}