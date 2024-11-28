using System.Drawing.Printing;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Cheeps;
using Chirp.Infrastructure.Authors;
using Chirp.Web.Pages.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Chirp.Core;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;

    private readonly SignInManager<Author> _signInManager;

    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();

    [BindProperty]
    public CheepFormModel Input { get; set; } = new();
    
    public int CurrentPage { get; set; }


    public PublicModel(ICheepService cheepService, IAuthorService authorService, SignInManager<Author> signInManager)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _signInManager = signInManager;
    }



    private void prepareContents()
    {
        var pageQuery = Request.Query["page"];
        CurrentPage = Convert.ToInt32(pageQuery) == 0 ? 1 : Convert.ToInt32(pageQuery);
        Cheeps = _cheepService.GetCheeps(CurrentPage,User.Identity?.Name!);
    }


    public ActionResult OnGet()
    {
        prepareContents();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            prepareContents();
            return Page();
        }
        else
        {
            Author ?author = await _signInManager.UserManager.GetUserAsync(User);
            if (author == null)
            {
                return Forbid("You are not logged in!!!");
            }
            _cheepService.AddCheep(author, Input.Message ?? throw new NullReferenceException());

            prepareContents();
            return RedirectToPage();
        }
    }
    public async Task<IActionResult> OnGetFollowAsync(string authorName)
    {
        _authorService.FollowAuthor(User.Identity?.Name!, authorName);
        prepareContents();
        return RedirectToPage();
    }
    public async Task<IActionResult> OnGetUnfollowAsync(string authorName)
    {
        _authorService.UnfollowAuthor(User.Identity?.Name!, authorName);
        prepareContents();
        return RedirectToPage();
    }
}
