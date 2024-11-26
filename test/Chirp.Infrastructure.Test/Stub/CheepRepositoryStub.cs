using Chirp.Core;

namespace Chirp.Infrastructure.Test.Stub;

public class CheepRepositoryStub : ICheepRepository
{
    private readonly List<Cheep> _cheeps = new();
    
    public Task AddCheep(Cheep cheep)
    {
        _cheeps.Add(cheep);
        return Task.CompletedTask;
    }
    
    public Task<List<CheepDTO>> GetCheepDTO(int page, string username)
    {
        var mockCheeps = new List<CheepDTO>
        {
            new CheepDTO("Author1", "This is a mock cheep 1", 1634567890, true),
            new CheepDTO("Author2", "This is a mock cheep 2", 1634567900,true),
            new CheepDTO("Author3", "This is a mock cheep 3", 1634567910, true)
        };
        
        return Task.FromResult(mockCheeps);
    }
    
    public Task<List<CheepDTO>> GetCheepDTOFromAuthor(int page, string authorName, string userName)
    {
        var mockCheeps = new List<CheepDTO>
        {
            new CheepDTO("Author1", "This is a mock cheep 1", 1634567890, true),
            new CheepDTO("Author2", "This is a mock cheep 2", 1634567900, true),
            new CheepDTO("Author3", "This is a mock cheep 3", 1634567910, true)
        };
        
        return Task.FromResult(mockCheeps);
    }
    public Task<List<CheepDTO>> GetCheepDTOFromMe(int page, string userName)
    {
        var mockCheeps = new List<CheepDTO>
        {
            new CheepDTO("Author1", "This is a mock cheep 1", 1634567890, true),
            new CheepDTO("Author2", "This is a mock cheep 2", 1634567900, true),
            new CheepDTO("Author3", "This is a mock cheep 3", 1634567910, true)
        };
        
        return Task.FromResult(mockCheeps);
    }
    public Task<Author> GetAuthorByName(string name)
    {
        var mockAuthor = new Author() { UserName = "Author1", Email = "au1@itu.dk", Cheeps = new List<Cheep>(), Following = new List<Author>(), Followers = new List<Author>() };
        return Task.FromResult(mockAuthor);
    }
    public Task<Author> GetAuthorByEmail(string email)
    {
        var mockAuthor = new Author() { UserName = "Author1", Email = "au1@itu.dk", Cheeps = new List<Cheep>(), Following = new List<Author>(), Followers = new List<Author>() };
        return Task.FromResult(mockAuthor);
    }
    
    public Task FollowAuthor(string followerName, string authorName)
    {
        return Task.CompletedTask;
    }
    public Task UnfollowAuthor(string followerName, string authorName)
    {
        return Task.CompletedTask;
    }
}