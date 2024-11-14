namespace Chirp.Test.Stub;
using Chirp.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chirp.Infrastructure;

public class CheepRepositoryStub : ICheepRepository
{
    private readonly List<Cheep> _cheeps = new();
    private readonly List<Author> _authors = new();
    
    public Task AddCheep(Cheep cheep)
    {
        _cheeps.Add(cheep);
        return Task.CompletedTask;
    }

    
    public Task AddAuthor(Author author)
    {
        _authors.Add(author);
        return Task.CompletedTask;
    }
    
    public Task<List<CheepDTO>> GetCheepDTO(int page)
    {
        var mockCheeps = new List<CheepDTO>
        {
            new CheepDTO("Author1", "This is a mock cheep 1", 1634567890),
            new CheepDTO("Author2", "This is a mock cheep 2", 1634567900),
            new CheepDTO("Author3", "This is a mock cheep 3", 1634567910)
        };
        
        return Task.FromResult(mockCheeps);
    }
    
    public Task<List<CheepDTO>> GetCheepDTOFromAuthor(int page, string authorName)
    {
        var mockCheeps = new List<CheepDTO>
        {
            new CheepDTO("Author1", "This is a mock cheep 1", 1634567890),
            new CheepDTO("Author2", "This is a mock cheep 2", 1634567900),
            new CheepDTO("Author3", "This is a mock cheep 3", 1634567910)
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
    
    public Task FollowAndUnfollowAuthor(string followerName, string authorName)
    {
        return Task.CompletedTask;
    }
}