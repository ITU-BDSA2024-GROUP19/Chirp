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

public interface IAuthorService
{
    Task<AuthorService.AddAuthorResult> AddAuthor(string userName, string email, string? password = null);
    void FollowAuthor(string followerName, string authorName);
    void UnfollowAuthor(string followerName, string authorName);
    List<Author> GetAllFollowingFromAuthor(string authorName);
    Author? GetAuthorByUsername(string username);
    public ICollection<Author> GetAuthorByEmail(string email);
}

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _repository;
    private readonly SignInManager<Author> _signInManager;
    private readonly UserManager<Author> _userManager;
    private readonly IUserStore<Author> _userStore;
    private readonly IUserEmailStore<Author> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;

    public AuthorService (
        IAuthorRepository repository,
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

    public record AddAuthorResult(Author User, IdentityResult IdentityResult);
    
    public async Task<AddAuthorResult> AddAuthor(string userName, string email, string? password = null)
    {
        Author user = CreateUser();
        await _userStore.SetUserNameAsync(user, userName, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
        var result = await _repository.AddAuthorAsync(user, password);
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
    
    public void FollowAuthor(string followerName, string authorName)
    {
        _repository.FollowAuthor(followerName, authorName);
    }
    public void UnfollowAuthor(string followerName, string authorName)
    {
        _repository.UnfollowAuthor(followerName, authorName);
    }

    public List<Author> GetAllFollowingFromAuthor(string authorName)
    {
        return _repository.GetAllFollowingFromAuthor(authorName).Result;
    }

    public Author? GetAuthorByUsername(string username)
    {
        return _repository.GetAuthorByUsernameAsync(username).Result;
    }

    public ICollection<Author> GetAuthorByEmail(string email)
    {
        return _repository.GetAuthorByEmailAsync(email).Result;
    }
}
