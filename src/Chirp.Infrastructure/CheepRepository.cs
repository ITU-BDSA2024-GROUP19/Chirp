using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure;

public interface ICheepRepository
{
    Task AddCheep(Cheep cheep);
    Task AddAuthor(Author author);
    Task<List<CheepDTO>> GetCheepDTO(int page);
    Task<List<CheepDTO>> GetCheepDTOFromAuthor(int page, string authorName);
    Task<Author> GetAuthorByName(string name);
    Task<Author> GetAuthorByEmail(string email);
    Task FollowAndUnfollowAuthor(string followerName, string authorName);
    
}

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    private const int CHEEPS_PER_PAGE = 32;

    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddCheep(Cheep cheep)
    {
        if (cheep.Text.Length > 160){
            throw new ValidationException("Cheep content must be less than 160 characters!");
        }
        else {
            _dbContext.Add(cheep);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task AddAuthor(Author author)
    {
        _dbContext.Add(author);
        await _dbContext.SaveChangesAsync();
    }

    public Task<Author> GetAuthorByName(string name)
    {
        var query = (from author in _dbContext.Authors where author.UserName == name select author);
        
        return query.FirstOrDefaultAsync()!;
    }
    
    public Task<Author> GetAuthorByEmail(string email)
    {
        var query = (from author in _dbContext.Authors where author.Email == email select author);
        
        return query.FirstOrDefaultAsync()!;
    }

    public Task<List<CheepDTO>> GetCheepDTO(int page)
    {
        var query = (from cheep in _dbContext.Cheeps
                orderby cheep.TimeStamp descending
                select new CheepDTO(cheep.Author.UserName ?? "", cheep.Text, (long)cheep.TimeStamp.Subtract(DateTime.UnixEpoch).TotalSeconds))
            //.Include(c => c.Author)
            .Skip((page - 1) * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        return query.ToListAsync();
    }
    
    public Task<List<CheepDTO>> GetCheepDTOFromAuthor(int page, string authorName)
    {
        var query = (from cheep in _dbContext.Cheeps
                where cheep.Author.UserName == authorName
                orderby cheep.TimeStamp descending
                select new CheepDTO(cheep.Author.UserName ?? "", cheep.Text, (long)cheep.TimeStamp.Subtract(DateTime.UnixEpoch).TotalSeconds))
            //.Include(c => c.Author)
            .Skip((page - 1) * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        return query.ToListAsync();
    }
    
    public async Task FollowAndUnfollowAuthor(string followerName, string authorName)
    {
        var follower = await GetAuthorByName(followerName);
        var author = await GetAuthorByName(authorName);
        if (follower == null || author == null)
        {
            throw new ValidationException("Author or follower not found!");
        }
        else if (follower.Following.Contains(author))
        {
            follower.Following.Remove(author);
        }
        else
        {
            follower.Following.Add(author);
            await _dbContext.SaveChangesAsync();
        }
    }
}