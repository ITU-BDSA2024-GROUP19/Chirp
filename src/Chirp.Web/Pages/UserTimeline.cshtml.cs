﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Cheeps;
using Chirp.Infrastructure.Authors;
using Microsoft.AspNetCore.Identity;
using Chirp.Core;
using Chirp.Web.Pages.Models;
using Chirp.Web.Pages.Shared.Models;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;

    private readonly SignInManager<Author> _signInManager;
    public List<CheepModel> Cheeps { get; set; } = new List<CheepModel>();

    [BindProperty]
    public CheepFormModel Input { get; set; } = new();
    
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
            new CheepModel(cheep.Author, cheep.Message, CheepModel.TimestampToCEST(cheep.Timestamp),cheep.IsFollowed));
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
            _cheepService.AddCheep(cheepauthor, Input.Message ?? throw new NullReferenceException());

            prepareContents(author);
            return RedirectToPage();
        }
    }
    
    public async Task<IActionResult> OnGetFollowAsync(string authorName) {
        string userName = User.Identity?.Name!;
        _authorService.FollowAuthor(userName, authorName);
        prepareContents(userName);
        return RedirectToPage("/UserTimeline", new { author = userName });
    }
    
    public async Task<IActionResult> OnPostFollowAsync(string authorName) {
        string userName = User.Identity?.Name!;
        _authorService.FollowAuthor(userName, authorName);
        prepareContents(userName);
        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnGetUnfollowAsync(string authorName)
    {
        string userName = User.Identity?.Name!;
        _authorService.UnfollowAuthor(userName, authorName);
        prepareContents(userName);
        return RedirectToPage("/UserTimeline", new { author = userName });
    }

    public async Task<IActionResult> OnPostUnfollowAsync(string authorName)
    {
        string userName = User.Identity?.Name!;
        _authorService.UnfollowAuthor(userName, authorName);
        prepareContents(userName);
        return RedirectToPage();
    }
}
