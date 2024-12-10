using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Cheeps;
using Chirp.Web.Pages.Models;
using Chirp.Web.Pages.Actions;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _cheepService;
    
    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();

    [BindProperty]
    public SendCheepModel.InputModel SendCheepInput { get; set; } = new();
    
    public int CurrentPage { get; set; }

    public UserTimelineModel(ICheepService cheepService)
    {
        _cheepService = cheepService;
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
            new CheepViewModel
            (
                cheep.Id, 
                cheep.Author, 
                cheep.Message, 
                CheepViewModel.TimestampToCEST(cheep.Timestamp),
                cheep.IsFollowed, 
                cheep.LikeCount, 
                cheep.IsLikedByUser
            ));
    }


    public ActionResult OnGet(string author)
    {
        prepareContents(author);
        return Page();
    }
}
