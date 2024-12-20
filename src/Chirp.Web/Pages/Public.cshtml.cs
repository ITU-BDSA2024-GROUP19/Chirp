﻿using Chirp.Infrastructure.Cheeps;
using Chirp.Web.Pages.Actions;
using Chirp.Web.Pages.Shared.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;

    public List<CheepViewModel> Cheeps { get; set; } = new List<CheepViewModel>();

    [BindProperty]
    public SendCheepModel.InputModel SendCheepInput { get; set; } = new();

    public int CurrentPage { get; set; }

    public PublicModel(ICheepService cheepService)
    {
        _cheepService = cheepService;
    }



    private void prepareContents()
    {
        var pageQuery = Request.Query["page"];
        CurrentPage = Convert.ToInt32(pageQuery) == 0 ? 1 : Convert.ToInt32(pageQuery);
        Cheeps = _cheepService.GetCheeps(CurrentPage,User.Identity?.Name!).ConvertAll(cheep => 
        new CheepViewModel(
            cheep.Id, 
            cheep.Author, 
            cheep.Message, 
            CheepViewModel.TimestampToCEST(cheep.Timestamp), 
            cheep.IsFollowed, 
            cheep.LikeCount, 
            cheep.IsLikedByUser,
            cheep.AuthorProfilePicture
        ));
    }


    public ActionResult OnGet()
    {
        prepareContents();
        return Page();
    }
}
