using System.Security.Principal;

using Chirp.Core;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.Extensions.Logging;

namespace Chirp.Infrastructure;

public interface IChirpAccountRepository
{
    Task<IdentityResult> AddAuthor(Author author, string password);
}

public class ChirpAccountRepository : IChirpAccountRepository
{
    private readonly ChirpDBContext _dbContext;
    
    private readonly IChirpAccountRepository _repository;
    private readonly SignInManager<Author> _signInManager;
    private readonly UserManager<Author> _userManager;
    private readonly IUserStore<Author> _userStore;
    private readonly IUserEmailStore<Author> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;

    public ChirpAccountRepository (
        ChirpDBContext dbContext,
        IChirpAccountRepository repository,
        UserManager<Author> userManager,
        IUserStore<Author> userStore,
        SignInManager<Author> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _repository = repository;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
    }
    
    public Task<IdentityResult> AddAuthor(Author user, string password)
    {
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