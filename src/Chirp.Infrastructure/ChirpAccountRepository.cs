using Chirp.Core;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.Extensions.Logging;

namespace Chirp.Infrastructure;

public interface IChirpAccountRepository
{
    Task AddAuthor(Author author);
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
    
    public async Task AddAuthor(Author author)
    {
        _dbContext.Add(author);
        await _dbContext.SaveChangesAsync();
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