// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Chirp.Core;
using Chirp.Infrastructure;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Chirp.Web.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<Author> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;
        private readonly ICheepService _cheepService;
        private readonly IAuthorService _authorService;
        public Dictionary<string, string> PersonalData { get; private set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public PersonalDataModel(
            UserManager<Author> userManager,
            ILogger<PersonalDataModel> logger,
            ICheepService cheepService,
            IAuthorService authorService)
        {
            _userManager = userManager;
            _logger = logger;
            _cheepService = cheepService;
            _authorService = authorService;
        }

        public async Task<IActionResult> OnGet(int pageid = 1)
        {
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var personalDataProps = typeof(Author).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                PersonalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                PersonalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }
            
            var cheeps = _cheepService.GetAllCheepsFromAuthor(user.UserName!, user.UserName!);
            PersonalData.Add("Total number of cheeps", cheeps.Count.ToString());
            for (int i = 0; i < cheeps.Count; i++)
            {
                PersonalData.Add($"Cheep {i + 1}", $"{cheeps[i].TimeStamp} {cheeps[i].Message}");
            }

            var follows = _authorService.GetAllFollowingFromAuthor(user.UserName!);
            PersonalData.Add("Total number of follows", follows.Count.ToString());
            for (int i = 0; i < follows.Count; i++)
            {
                PersonalData.Add($"Follow {i + 1}", $"{follows[i].UserName}");
            }

            //bits used from this: https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.todictionary?view=net-9.0
            int pageSize = 32;
            var paginatedData = PersonalData.Skip((pageid - 1) * pageSize).Take(pageSize).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            CurrentPage = pageid;
            TotalPages = (int)Math.Ceiling(PersonalData.Count / (double)pageSize);
            PersonalData = paginatedData;
            
            return Page();
        }
    }
}
