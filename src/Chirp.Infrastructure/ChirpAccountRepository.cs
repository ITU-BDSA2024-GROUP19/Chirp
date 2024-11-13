using Chirp.Core;

namespace Chirp.Infrastructure;

public interface IChirpAccountRepository
{
    Task AddAuthor(Author author);
}

public class ChirpAccountRepository : IChirpAccountRepository
{
    private readonly ChirpDBContext _dbContext;
    
    public ChirpAccountRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAuthor(Author author)
    {
        _dbContext.Add(author);
        await _dbContext.SaveChangesAsync();
    }
}