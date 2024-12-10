using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Cheeps;

namespace Chirp.Web.Pages.Actions;



public class AuthorLikesModel : PageModel 
{
    private readonly ICheepService _cheepService;


    public AuthorLikesModel(ICheepService cheepService) {
        _cheepService = cheepService;
    }

    public IActionResult OnGet()
    {
        return new StatusCodeResult(405); // Method not allowed
    }

    public IActionResult OnPostLike(int cheepId, string? returnUrl = null)
    {
        Console.WriteLine("AuthorLikes OnPostLike(...) called");
        Console.WriteLine(User.Identity?.Name! + " wants to like cheep: " + cheepId);
        returnUrl ??= Url.Content("~/");

        _cheepService.LikeCheep(cheepId, User.Identity?.Name!);
        return LocalRedirect(returnUrl);
    }

    public IActionResult OnPostRemoveLike(int cheepId, string? returnUrl = null)
    {
        Console.WriteLine("AuthorLikes OnPostRemoveLike(...) called");
        Console.WriteLine(User.Identity?.Name! + " wants to remove like cheep: " + cheepId);
        returnUrl ??= Url.Content("~/");

        _cheepService.RemoveLikeCheep(cheepId, User.Identity?.Name!);
        return LocalRedirect(returnUrl);
    }
}