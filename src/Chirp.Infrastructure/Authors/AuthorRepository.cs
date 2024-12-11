using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

using Chirp.Core;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Authors;

public interface IAuthorRepository
{
    Task<IdentityResult> AddAuthorAsync(Author user, string? password = null);
    Task FollowAuthor(string followerName, string authorName);
    Task UnfollowAuthor(string followerName, string authorName);
    Task<List<Author>> GetAllFollowingFromAuthor(string authorName);
    Task<Author?> GetAuthorByUsernameAsync(string username);
    Task<ICollection<Author>> GetAuthorByEmailAsync(string email);
    Task UpdateProfilePicture(string username, Stream profilePicture);
    Task<string> GetProfilePicture(string username);
    Task DeleteProfilePicture(string username);
    
    
}

public class AuthorRepository : IAuthorRepository
{
    private readonly UserManager<Author> _userManager;
    private readonly IUserStore<Author> _userStore;
    private readonly ChirpDBContext _dbContext;
    private readonly BlobContainerClient _blobContainerClient;

    public AuthorRepository (UserManager<Author> userManager, IUserStore<Author> userStore, ChirpDBContext dbContext, BlobServiceClient blobServiceClient)
    {
        _userManager = userManager;
        _userStore = userStore;
        _dbContext = dbContext;
        _blobContainerClient = blobServiceClient.GetBlobContainerClient("profile-pictures");
        _blobContainerClient.CreateIfNotExists();
        
    }
    
    public Task<IdentityResult> AddAuthorAsync(Author user, string? password = null)
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
    
    public async Task FollowAuthor(string followerName, string authorName)
    {
        var follower = await _dbContext.Authors
            .Include(a => a.Following)
            .FirstOrDefaultAsync(a => a.UserName == followerName);

        var author = await _dbContext.Authors
            .Include(a => a.Followers)
            .FirstOrDefaultAsync(a => a.UserName == authorName);

        if (follower == null || author == null)
        {
            throw new ArgumentException("User or author does not exist.");
        }
        
        follower.Following.Add(author);

        await _dbContext.SaveChangesAsync();
    }
    public async Task UnfollowAuthor(string followerName, string authorName)
    {
        var follower = await _dbContext.Authors
            .Include(a => a.Following)
            .FirstOrDefaultAsync(a => a.UserName == followerName);

        var author = await _dbContext.Authors
            .Include(a => a.Followers)
            .FirstOrDefaultAsync(a => a.UserName == authorName);

        if (follower == null || author == null)
        {
            throw new ArgumentException("User or author does not exist");
        }
        
        follower.Following.Remove(author);

        await _dbContext.SaveChangesAsync();
    }

    public Task<List<Author>> GetAllFollowingFromAuthor(string authorName)
    {
        var query = _dbContext.Authors
            .Where(a => a.UserName == authorName)
            .SelectMany(a => a.Following);

        return query.ToListAsync();
    }

    public async Task<Author?> GetAuthorByUsernameAsync(string username)
    {
        return await _dbContext.Authors.FirstOrDefaultAsync(author => author.UserName == username);
    }

    public async Task<ICollection<Author>> GetAuthorByEmailAsync(string email)
    {
        return await _dbContext.Authors.Where(author => author.Email == email).ToListAsync();
    }
    
    public async Task UpdateProfilePicture(string username, Stream profilePicture)
    {
        var author = await _dbContext.Authors
            .FirstOrDefaultAsync(a => a.UserName == username);
        if (author == null)
        {
            throw new ArgumentException("User does not exist.");
        }
        
        BlobClient blobClient = _blobContainerClient.GetBlobClient(username);
        blobClient.Upload(profilePicture, true);
        
        var url =  blobClient.Uri.ToString();
        author.ProfilePicture = url;
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<string> GetProfilePicture(string username)
    {
        var author = await _dbContext.Authors
            .FirstOrDefaultAsync(a => a.UserName == username);
        if (author == null)
        {
            throw new ArgumentException("User does not exist.");
        }
        var imageUrl = author.ProfilePicture;
        if (imageUrl != "default.jpg")
        {
            var blobClient = _blobContainerClient.GetBlobClient(username);
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _blobContainerClient.Name,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
            };
                    
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    
            var sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }
        return imageUrl;
    }
    
    public async Task DeleteProfilePicture(string username)
    {
        var author = await _dbContext.Authors
            .FirstOrDefaultAsync(a => a.UserName == username);
        if (author == null)
        {
            throw new ArgumentException("User does not exist.");
        }
        
        BlobClient blobClient = _blobContainerClient.GetBlobClient(username);
        
        author.ProfilePicture = "default.jpg";
        
        await Task.WhenAll(blobClient.DeleteIfExistsAsync(),_dbContext.SaveChangesAsync());
    }
}
