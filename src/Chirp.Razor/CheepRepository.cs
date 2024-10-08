using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public interface ICheepRepository
{
    Task<List<Cheep>> GetCheeps(int page);
    Task<List<Cheep>> GetCheepsFromAuthor(string authorName, int page);
    Task AddCheep(Cheep cheep);
    Task AddAuthor(Author author);
}

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    private const int CHEEPS_PER_PAGE = 32;

    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<List<Cheep>> GetCheeps(int page)
    {
        var query = (from cheep in _dbContext.Cheeps
                orderby cheep.TimeStamp descending
                select cheep)
            .Include(c => c.Author)
            .Skip((page - 1) * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        return query.ToListAsync();
    }

    public async Task<List<Cheep>> GetCheepsFromAuthor(string authorName, int page)
    {
        var query = (from cheep in _dbContext.Cheeps 
                where cheep.Author.Name == authorName 
                orderby cheep.TimeStamp descending
                select cheep)
            .Include(c => c.Author)
            .Skip((page - 1) * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE); 
        return await query.ToListAsync();
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
}