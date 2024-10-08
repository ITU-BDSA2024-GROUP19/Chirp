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
    
    public async Task<List<Cheep>> GetCheeps(int page)
    {
        var query = (from cheep in _dbContext.Cheeps 
                orderby cheep.TimeStamp descending
                select cheep)
            .Include(c => c.Author)
            .Skip(page * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<List<Cheep>> GetCheepsFromAuthor(string authorName, int page)
    {
        var query = (from cheep in _dbContext.Cheeps 
                where cheep.Author.Name == authorName 
                orderby cheep.TimeStamp descending
                select cheep)
            .Include(c => c.Author)
            .Skip(page * CHEEPS_PER_PAGE)
            .Take(CHEEPS_PER_PAGE);
        var result = await query.ToListAsync();
        return result;
    }

    public async Task AddCheep(Cheep cheep)
    {
        throw new NotImplementedException();
    }

    public async Task AddAuthor(Author author)
    {
        throw new NotImplementedException();
    }
}