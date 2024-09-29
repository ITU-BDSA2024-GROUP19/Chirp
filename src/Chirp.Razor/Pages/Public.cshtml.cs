using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        var pageQuery = Request.Query["page"];
        Cheeps = _service.GetCheeps(Convert.ToInt32(pageQuery));
        return Page();
    }
}
