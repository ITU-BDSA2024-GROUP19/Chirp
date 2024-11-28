// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Chirp.Core;
using Chirp.Infrastructure.Cheeps;
using Chirp.Infrastructure.Authors;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Chirp.Web.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<Author> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;
        private readonly ICheepService _cheepService;
        private readonly IAuthorService _authorService;


        public DownloadPersonalDataModel(
            UserManager<Author> userManager,
            ILogger<DownloadPersonalDataModel> logger,
            ICheepService cheepService,
            IAuthorService authorService)
        {
            _userManager = userManager;
            _logger = logger;
            _cheepService = cheepService;
            _authorService = authorService;
        }

        public IActionResult OnGet()
        {
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, object>();
            var personalDataProps = typeof(Author).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));
            
            var cheeps = _cheepService.GetAllCheepsFromAuthor(user.UserName, user.UserName);
            var cheepsList = new List<Dictionary<string, string>>();
            for (int i = 0; i < cheeps.Count; i++)
            {
                cheepsList.Add(new Dictionary<string, string>
                {
                    { "Message", cheeps[i].Message },
                    { "TimeStamp", cheeps[i].TimeStamp.ToString() },
                    { "Author", cheeps[i].Author.ToString() }
                });
            }
            personalData.Add("Cheeps", cheepsList);
            
            var follows = _authorService.GetAllFollowingFromAuthor(user.UserName!);
            var followList = new List<Dictionary<string, string>>();
            for (int i = 0; i < follows.Count; i++)
            {
                followList.Add(new Dictionary<string, string>
                {
                    { "Follow", follows[i].UserName }
                });
            }
            personalData.Add("Follows", followList);

            Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            
            var personalDataJson = JsonSerializer.SerializeToUtf8Bytes(personalData);
            return new FileContentResult(personalDataJson, "application/json");
        }
    }
}
