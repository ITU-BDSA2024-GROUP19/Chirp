using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Chirp.Core;
using Chirp.Web.Pages.Shared.Models;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;

    private readonly SignInManager<Author> _signInManager;
    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();

    [BindProperty]
    public CheepFormModel Input { get; set; } = new();
    
    public int CurrentPage { get; set; }

    public UserTimelineModel(ICheepService service, SignInManager<Author> signInManager)
    {
        _service = service;
        _signInManager = signInManager;
    }

    private void prepareContents(string author)
    {
        var pageQuery = Request.Query["page"];
        CurrentPage = Convert.ToInt32(pageQuery) == 0 ? 1 : Convert.ToInt32(pageQuery);
        Cheeps = _service.GetCheepsFromAuthor(CurrentPage, author);
    }


    public ActionResult OnGet(string author)
    {
        prepareContents(author);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string author)
    {
        if (!ModelState.IsValid)
        {
            prepareContents(author);
            return Page();
        }
        else
        {
            Author ?cheepauthor = await _signInManager.UserManager.GetUserAsync(User);
            if (cheepauthor == null)
            {
                return Forbid("You are not logged in!!!");
            }
            _service.AddCheep(cheepauthor, Input.Message ?? throw new NullReferenceException());

            prepareContents(author);
            return RedirectToPage();
        }
    }

}
