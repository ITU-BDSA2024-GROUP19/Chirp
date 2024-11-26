using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using System.ComponentModel.DataAnnotations;

using CheepDTO = Chirp.Core.CheepDTO;

namespace Chirp.Infrastructure;

public interface ICheepRepository
{
    Task AddCheep(Cheep cheep);
    Task<List<CheepDTO>> GetCheepDTO(int page, string userName);
    Task<List<CheepDTO>> GetCheepDTOFromAuthor(int page, string authorName, string userName);
    Task<List<CheepDTO>> GetCheepDTOFromMe(int page, string userName);
    Task<Author> GetAuthorByName(string name);
    Task<Author> GetAuthorByEmail(string email);
    Task FollowAuthor(string followerName, string authorName);
    Task UnfollowAuthor(string followerName, string authorName);
    Task<List<CheepDTO>> GetAllCheepDTOFromAuthor(string author, string userName);
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

    public Task<List<CheepDTO>> GetCheepDTO(int page, string userName)
    {
        var user = _dbContext.Authors.FirstOrDefaultAsync(a => a.UserName == userName);
        var query = (from cheep in _dbContext.Cheeps
                orderby cheep.TimeStamp descending
                select new CheepDTO(cheep.Author.UserName ?? "", cheep.Text, (long)cheep.TimeStamp.Subtract(DateTime.UnixEpoch).TotalSeconds, user.Result != null && cheep.Author.Followers.Contains(user.Result)))
            .Skip((page - 1) * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        return query.ToListAsync();
    }
    
    public Task<List<CheepDTO>> GetCheepDTOFromAuthor(int page, string authorName, string userName)
    {
        var user = _dbContext.Authors.FirstOrDefaultAsync(a => a.UserName == userName);
        var query = (from cheep in _dbContext.Cheeps
                where cheep.Author.UserName == authorName
                orderby cheep.TimeStamp descending
                select new CheepDTO(cheep.Author.UserName ?? "", cheep.Text, (long)cheep.TimeStamp.Subtract(DateTime.UnixEpoch).TotalSeconds, user.Result != null && cheep.Author.Followers.Contains(user.Result)))
            .Skip((page - 1) * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        return query.ToListAsync();
    }
    public Task<List<CheepDTO>> GetCheepDTOFromMe(int page, string userName)
    {
        var user = _dbContext.Authors.FirstOrDefaultAsync(a => a.UserName == userName);
        var query = (from cheep in _dbContext.Cheeps
                where cheep.Author.UserName == userName || 
                      cheep.Author.Followers.Any(f => f.UserName == userName)
                orderby cheep.TimeStamp descending
                select new CheepDTO(cheep.Author.UserName ?? "", cheep.Text, (long)cheep.TimeStamp.Subtract(DateTime.UnixEpoch).TotalSeconds, user.Result != null && cheep.Author.Followers.Contains(user.Result)))
            .Skip((page - 1) * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        return query.ToListAsync();
    }
    
    // Removed cheep.Author.Followers.Any(f => f.UserName == author) from the query and the .Skip(0) and .Take(CHEEPS_PER_PAGE); as no pagination is needed is this method.
    public Task<List<CheepDTO>> GetAllCheepDTOFromAuthor(string author, string userName)
    {
        var user = _dbContext.Authors.FirstOrDefaultAsync(a => a.UserName == userName);
        var query = from cheep in _dbContext.Cheeps
            where cheep.Author.UserName == author
            orderby cheep.TimeStamp descending
            select new CheepDTO(
                cheep.Author.UserName ?? "", 
                cheep.Text, 
                (long)cheep.TimeStamp.Subtract(DateTime.UnixEpoch).TotalSeconds, 
                user.Result != null && cheep.Author.Followers.Contains(user.Result)
            );
        return query.ToListAsync();
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
}