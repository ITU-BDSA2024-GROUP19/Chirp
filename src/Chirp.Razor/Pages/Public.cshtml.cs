using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _service;
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    
    public int CurrentPage { get; set; }
    public PublicModel(ICheepRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        var pageQuery = Request.Query["page"];
        CurrentPage = Convert.ToInt32(pageQuery) == 0 ? 1 : Convert.ToInt32(pageQuery);
        Cheeps = _service.GetCheeps(CurrentPage).Result;
        return Page();
    }
}
