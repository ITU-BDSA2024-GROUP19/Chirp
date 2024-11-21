using System.Security.Principal;

using Chirp.Core;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.Extensions.Logging;

namespace Chirp.Infrastructure;

public interface IAuthorRepository
{
    Task<IdentityResult> AddAuthor(Author user, string? password = null);
}

public class AuthorRepository : IAuthorRepository
{
    private readonly UserManager<Author> _userManager;
    private readonly IUserStore<Author> _userStore;


    public AuthorRepository (UserManager<Author> userManager, IUserStore<Author> userStore)
    {
        _userManager = userManager;
        _userStore = userStore;
    }
    
    public Task<IdentityResult> AddAuthor(Author user, string? password = null)
    {
        if (password == null) {
            return _userManager.CreateAsync(user);
        }
        return _userManager.CreateAsync(user, password);
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