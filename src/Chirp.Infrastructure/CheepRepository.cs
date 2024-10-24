using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure;

public interface ICheepRepository
{
    Task AddCheep(Cheep cheep);
    Task AddAuthor(Author author);
    Task<List<CheepDTO>> GetCheepDTO(int page);
    Task<List<CheepDTO>> GetCheepDTOFromAuthor(int page, string authorName);
    Task<Author> GetAuthorByName(string name);
    Task<Author> GetAuthorByEmail(string email);
    
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
        _dbContext.Add(cheep);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddAuthor(Author author)
    {
        _dbContext.Add(author);
        await _dbContext.SaveChangesAsync();
    }

    public Task<Author> GetAuthorByName(string name)
    {
        var query = (from author in _dbContext.Authors where author.Name == name select author);
        
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
                select new CheepDTO(cheep.Author.Name, cheep.Text, (long)cheep.TimeStamp.Subtract(DateTime.UnixEpoch).TotalSeconds))
            //.Include(c => c.Author)
            .Skip((page - 1) * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        return query.ToListAsync();
    }
    
    public Task<List<CheepDTO>> GetCheepDTOFromAuthor(int page, string authorName)
    {
        var query = (from cheep in _dbContext.Cheeps
                where cheep.Author.Name == authorName
                orderby cheep.TimeStamp descending
                select new CheepDTO(cheep.Author.Name, cheep.Text, (long)cheep.TimeStamp.Subtract(DateTime.UnixEpoch).TotalSeconds))
            //.Include(c => c.Author)
            .Skip((page - 1) * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        return query.ToListAsync();
    }
    
}