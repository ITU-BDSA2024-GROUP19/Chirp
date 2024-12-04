using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace Chirp.Infrastructure.Cheeps;

public record CheepDto(
    int Id,
    string Author, 
    string Message, 
    long Timestamp, 
    bool IsFollowed, 
    int LikeCount,
    bool IsLikedByUser,
    string AuthorProfilePicture
);
public interface ICheepRepository
{
    Task AddCheep(Cheep cheep);
    Task<Cheep?> GetCheepWithLikes(int cheepId);
    Task UpdateCheep(Cheep cheep);
    Task<List<CheepDto>> GetCheepDTO(int page, string userName);
    Task<List<CheepDto>> GetCheepDTOFromAuthor(int page, string authorName, string userName);
    Task<List<CheepDto>> GetCheepDTOFromMe(int page, string userName);
    Task<Author> GetAuthorByName(string name);
    Task<Author> GetAuthorByEmail(string email);
    Task<List<CheepDto>> GetAllCheepDTOFromAuthor(string author, string userName);
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
        if (cheep.Text.Length > 160)
        {
            throw new ValidationException("Cheep content must be less than 160 characters!");
        }
        else 
        {
            _dbContext.Add(cheep);
            await _dbContext.SaveChangesAsync();
        }
    }

    public Task<Cheep?> GetCheepWithLikes(int cheepId)
    {
        return _dbContext.Cheeps
            .Include(c => c.Likes)
            .FirstOrDefaultAsync(c => c.CheepId == cheepId);
    }

    public async Task UpdateCheep(Cheep cheep)
    {
        //TODO: Add restrictions/validations
        _dbContext.Update(cheep);
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

    public async Task<List<CheepDto>> GetCheepDTO(int page, string userName)
    {
        var user = await _dbContext.Authors.FirstOrDefaultAsync(a => a.UserName == userName);
        var query = (from cheep in _dbContext.Cheeps
                orderby cheep.TimeStamp descending
                select new CheepDto(
                    cheep.CheepId,
                    cheep.Author.UserName ?? "", 
                    cheep.Text, 
                    (long)cheep.TimeStamp.Subtract(DateTime.UnixEpoch).TotalSeconds, 
                    user != null && cheep.Author.Followers.Contains(user),
                    cheep.Likes.Count,
                    user != null && cheep.Likes.Contains(user),
                    cheep.Author.ProfilePicture
                ))
            .Skip((page - 1) * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        return await query.ToListAsync();
    }
    
    public async Task<List<CheepDto>> GetCheepDTOFromAuthor(int page, string authorName, string userName)
    {
        var user = await _dbContext.Authors.FirstOrDefaultAsync(a => a.UserName == userName);
        var query = (from cheep in _dbContext.Cheeps
                where cheep.Author.UserName == authorName
                orderby cheep.TimeStamp descending
                select new CheepDto(
                    cheep.CheepId,
                    cheep.Author.UserName ?? "", 
                    cheep.Text, 
                    (long)cheep.TimeStamp.Subtract(DateTime.UnixEpoch).TotalSeconds, 
                    user != null && cheep.Author.Followers.Contains(user),
                    cheep.Likes.Count,
                    user != null && cheep.Likes.Contains(user),
                    cheep.Author.ProfilePicture
                    ))
            .Skip((page - 1) * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        return await query.ToListAsync();
    }
    public async Task<List<CheepDto>> GetCheepDTOFromMe(int page, string userName)
    {
        var user = await _dbContext.Authors.FirstOrDefaultAsync(a => a.UserName == userName);
        var query = (from cheep in _dbContext.Cheeps
                where cheep.Author.UserName == userName || 
                      cheep.Author.Followers.Any(f => f.UserName == userName)
                orderby cheep.TimeStamp descending
                select new CheepDto(
                    cheep.CheepId,
                    cheep.Author.UserName ?? "", 
                    cheep.Text, 
                    (long)cheep.TimeStamp.Subtract(DateTime.UnixEpoch).TotalSeconds, 
                    user != null && cheep.Author.Followers.Contains(user),
                    cheep.Likes.Count,
                    user != null && cheep.Likes.Contains(user),
                    cheep.Author.ProfilePicture
                    ))
            .Skip((page - 1) * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        return await query.ToListAsync();
    }
    
    // Removed cheep.Author.Followers.Any(f => f.UserName == author) from the query and the .Skip(0) and .Take(CHEEPS_PER_PAGE); as no pagination is needed is this method.
    public async Task<List<CheepDto>> GetAllCheepDTOFromAuthor(string author, string userName)
    {
        var user = await _dbContext.Authors.FirstOrDefaultAsync(a => a.UserName == userName);
        var query = from cheep in _dbContext.Cheeps
            where cheep.Author.UserName == author
            orderby cheep.TimeStamp ascending
            select new CheepDto(
                cheep.CheepId,
                cheep.Author.UserName ?? "", 
                cheep.Text, 
                (long)cheep.TimeStamp.Subtract(DateTime.UnixEpoch).TotalSeconds, 
                user != null && cheep.Author.Followers.Contains(user),
                cheep.Likes.Count,
                user != null && cheep.Likes.Contains(user),
                cheep.Author.ProfilePicture
            );
        return await query.ToListAsync();
    }
}