// Re-uses substantial parts of the Identity page models code, for which this license applies.
//
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

using Chirp.Core;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.Extensions.Logging;

namespace Chirp.Infrastructure;

public interface IChirpAccountService
{
    Task<ChirpAccountService.AddAuthorResult> AddAuthor(ChirpAccountService.NewAccountInputModel input);
}

public class ChirpAccountService : IChirpAccountService
{
    private readonly IChirpAccountRepository _repository;
    private readonly SignInManager<Author> _signInManager;
    private readonly UserManager<Author> _userManager;
    private readonly IUserStore<Author> _userStore;
    private readonly IUserEmailStore<Author> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;

    public ChirpAccountService (
        IChirpAccountRepository repository,
        UserManager<Author> userManager,
        IUserStore<Author> userStore,
        SignInManager<Author> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender)
    {
        _repository = repository;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
    }

    public class NewAccountInputModel
    {
        public required string UserName { get; set; }
        
        public required string Email { get; set; }
        
        public required string Password { get; set; }
    }

    public record AddAuthorResult(Author User, IdentityResult Result);
    
    public async Task<AddAuthorResult> AddAuthor(NewAccountInputModel input)
    {
        Author user = CreateUser();
        await _userStore.SetUserNameAsync(user, input.UserName, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, input.Email, CancellationToken.None);
        var result = await _repository.AddAuthor(user, input.Password);
        return new AddAuthorResult(user, result);
    }
    
    private Author CreateUser()
    {
        try
        {
            return Activator.CreateInstance<Author>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(Author)}'. " + 
                                                $"Ensure that '{nameof(Author)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                                                $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
        }
    }
    
    private IUserEmailStore<Author> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<Author>)_userStore;
    }
}