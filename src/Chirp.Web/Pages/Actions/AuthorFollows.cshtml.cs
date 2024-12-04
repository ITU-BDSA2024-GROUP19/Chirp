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
    public class InputModel
    {
        [Required]
        public string? AuthorName { get; set; }
    }

    private readonly IAuthorService _authorService;
    private readonly SignInManager<Author> _signInManager;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public AuthorFollowsModel(
        IAuthorService authorService, 
        SignInManager<Author> signInManager)
    {
        _authorService = authorService;
        _signInManager = signInManager;
    }

    //https://www.aspsnippets.com/Articles/3165/Using-the-OnPost-handler-method-in-ASPNet-Core-Razor-Pages/#google_vignette
    public async Task<IActionResult> OnPostFollowAsync(string? returnUrl = null)
    {
        Console.WriteLine(User.Identity?.Name! + " will follow author: " + Input.AuthorName);
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state");
        }

        if (await _signInManager.UserManager.GetUserAsync(User) == null)
        {
            return Forbid("You are not logged in!!!");
        }

        _authorService.FollowAuthor(User.Identity?.Name!, Input.AuthorName ?? throw new NullReferenceException());
        return LocalRedirect(returnUrl);
    }

    public async Task<IActionResult> OnPostUnfollowAsync(string? returnUrl = null)
    {
        Console.WriteLine(User.Identity?.Name! + " will UNfollow author: " + Input.AuthorName);
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state");
        }

        if (await _signInManager.UserManager.GetUserAsync(User) == null)
        {
            return Forbid("You are not logged in!!!");
        }

        _authorService.UnfollowAuthor(User.Identity?.Name!, Input.AuthorName ?? throw new NullReferenceException());
        return LocalRedirect(returnUrl);
    }
}