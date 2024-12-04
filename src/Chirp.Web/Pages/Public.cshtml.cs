using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Cheeps;
using Chirp.Infrastructure.Authors;
using Chirp.Web.Pages.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Chirp.Core;
using Chirp.Web.Pages.Models;
using Chirp.Web.Pages.Actions;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    private readonly SignInManager<Author> _signInManager;

    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();

    [BindProperty]
    public SendCheepModel.InputModel SendCheepInput { get; set; } = new();
    
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
        Cheeps = _cheepService.GetCheeps(CurrentPage,User.Identity?.Name!).ConvertAll(cheep => 
        new CheepViewModel(cheep.Author, cheep.Message, CheepViewModel.TimestampToCEST(cheep.Timestamp),cheep.IsFollowed));
    }


    public ActionResult OnGet()
    {
        prepareContents();
        return Page();
    }
}
