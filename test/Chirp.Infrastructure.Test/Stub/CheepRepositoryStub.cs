using Chirp.Core;
using Chirp.Infrastructure.Cheeps;

namespace Chirp.Infrastructure.Test.Stub;

public class CheepRepositoryStub : ICheepRepository
{
    private readonly List<Cheep> _cheeps = new();
    
    public Task AddCheep(Cheep cheep)
    {
        _cheeps.Add(cheep);
        return Task.CompletedTask;
    }
    
    public Task<List<CheepDto>> GetCheepDTO(int page, string username)
    {
        var mockCheeps = new List<CheepDto>
        {
            new CheepDto(1, "Author1", "This is a mock cheep 1", 1634567890, true, 0, false,"https://www.example.com"),
            new CheepDto(2, "Author3", "This is a mock cheep 3", 1634567910, true, 0, false,"https://www.example.com"),
            new CheepDto(3, "Author2", "This is a mock cheep 2", 1634567900,true, 0, false,"https://www.example.com"),
        };
        
        return Task.FromResult(mockCheeps);
    }
    
    public Task<List<CheepDto>> GetCheepDTOFromAuthor(int page, string authorName, string userName)
    {
        var mockCheeps = new List<CheepDto>
        {
            new CheepDto(1, "Author1", "This is a mock cheep 1", 1634567890, true, 0, false,"https://www.example.com"),
            new CheepDto(2, "Author2", "This is a mock cheep 2", 1634567900, true, 0, false"https://www.example.com"),
            new CheepDto(3, "Author3", "This is a mock cheep 3", 1634567910, true, 0, false,"https://www.example.com")
        };
        
        return Task.FromResult(mockCheeps);
    }
    public Task<List<CheepDto>> GetCheepDTOFromMe(int page, string userName)
    {
        var mockCheeps = new List<CheepDto>
        {
            new CheepDto(1, "Author1", "This is a mock cheep 1", 1634567890, true, 0, false,"https://www.example.com"),
            new CheepDto(2, "Author2", "This is a mock cheep 2", 1634567900, true, 0, false,"https://www.example.com"),
            new CheepDto(3, "Author3", "This is a mock cheep 3", 1634567910, true, 0, false,"https://www.example.com")
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

    public Task<List<CheepDto>> GetAllCheepDTOFromAuthor(string author, string userName)
    {
        return (Task<List<CheepDto>>)Task.CompletedTask;
    }
}