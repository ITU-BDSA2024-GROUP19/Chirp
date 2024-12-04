using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Cheeps;
using Chirp.Infrastructure.Authors;
using Chirp.Web.Pages.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Chirp.Core;
using Chirp.Web.Pages.Models;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages.Actions;

public class AuthorFollowsModel : PageModel
{
    private readonly IAuthorService _authorService;

    public AuthorFollowsModel(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    public IActionResult OnGet()
    {
        return new StatusCodeResult(405); // Method not allowed
    }

    //https://www.aspsnippets.com/Articles/3165/Using-the-OnPost-handler-method-in-ASPNet-Core-Razor-Pages/#google_vignette
    public IActionResult OnPostFollow(string authorName, string? returnUrl = null)
    {
        Console.WriteLine("AuthorFollows OnPostFollow: " + User.Identity?.Name! + " will follow author: " + authorName);
        returnUrl ??= Url.Content("~/");

        _authorService.FollowAuthor(User.Identity?.Name!, authorName);
        return LocalRedirect(returnUrl);
    }

    public IActionResult OnPostUnfollow(string authorName, string? returnUrl = null)
    {
        Console.WriteLine("AuthorFollows OnPostUnfollow: " + User.Identity?.Name! + " will UNfollow author: " + authorName);
        returnUrl ??= Url.Content("~/");

        _authorService.UnfollowAuthor(User.Identity?.Name!, authorName);
        return LocalRedirect(returnUrl);
    }
}