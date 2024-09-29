using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author)
    {
        var pageQuery = Request.Query["page"];
        Cheeps = _service.GetCheepsFromAuthor(author,Convert.ToInt32(pageQuery));
        return Page();
    }
}
