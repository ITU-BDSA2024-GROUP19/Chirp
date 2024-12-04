using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Cheeps;
using Chirp.Infrastructure.Authors;
using Microsoft.AspNetCore.Identity;
using Chirp.Core;
using Chirp.Web.Pages.Models;
using Chirp.Web.Pages.Actions;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;

    private readonly SignInManager<Author> _signInManager;
    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();

    [BindProperty]
    public SendCheepModel.InputModel SendCheepInput { get; set; } = new();
    
    public int CurrentPage { get; set; }

    public UserTimelineModel(ICheepService cheepService, IAuthorService authorService, SignInManager<Author> signInManager)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _signInManager = signInManager;
    }

    private void prepareContents(string author)
    {
        var pageQuery = Request.Query["page"];
        CurrentPage = Convert.ToInt32(pageQuery) == 0 ? 1 : Convert.ToInt32(pageQuery);
        var userName = User.Identity?.Name!;
        List<CheepDto> cheepData;
        if (author == userName)
        {
            cheepData = _cheepService.GetCheepsFromMe(CurrentPage, userName);
        }
        else
        {
            cheepData = _cheepService.GetCheepsFromAuthor(CurrentPage, author, userName);
        }
        Cheeps = cheepData.ConvertAll(cheep => 
            new CheepViewModel(cheep.Author, cheep.Message, CheepViewModel.TimestampToCEST(cheep.Timestamp),cheep.IsFollowed));
    }


    public ActionResult OnGet(string author)
    {
        prepareContents(author);
        return Page();
    }
    
    public async Task<IActionResult> OnPostFollowAsync(string authorName) {
        string userName = User.Identity?.Name!;
        _authorService.FollowAuthor(userName, authorName);
        prepareContents(userName);
        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostUnfollowAsync(string authorName)
    {
        string userName = User.Identity?.Name!;
        _authorService.UnfollowAuthor(userName, authorName);
        prepareContents(userName);
        return RedirectToPage();
    }
}
