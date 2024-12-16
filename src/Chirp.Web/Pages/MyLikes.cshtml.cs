using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Cheeps;
using Chirp.Web.Pages.Shared.Models;
using Chirp.Web.Pages.Actions;

namespace Chirp.Web.Pages;

public class MyLikesModel : PageModel
{
    private readonly ICheepService _cheepService;
    
    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();

    [BindProperty]
    public SendCheepModel.InputModel SendCheepInput { get; set; } = new();
    
    public int CurrentPage { get; set; }

    public MyLikesModel(ICheepService cheepService)
    {
        _cheepService = cheepService;
    }

    private void prepareContents(string author)
    {
        var pageQuery = Request.Query["page"];
        CurrentPage = Convert.ToInt32(pageQuery) == 0 ? 1 : Convert.ToInt32(pageQuery);
        var userName = User.Identity?.Name!;
        List<CheepDto> cheepData;
        cheepData = _cheepService.GetCheepsFromAuthorLikes(CurrentPage, userName);
        Cheeps = cheepData.ConvertAll(cheep => 
            new CheepViewModel
            (
                cheep.Id, 
                cheep.Author, 
                cheep.Message, 
                CheepViewModel.TimestampToCEST(cheep.Timestamp),
                cheep.IsFollowed, 
                cheep.LikeCount, 
                cheep.IsLikedByUser,
                cheep.AuthorProfilePicture
            ));
        Console.WriteLine(Cheeps.Count);
    }


    public ActionResult OnGet(string author)
    {
        prepareContents(author);
        return Page();
    }
}
