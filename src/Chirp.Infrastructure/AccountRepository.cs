using Chirp.Core;

namespace Chirp.Infrastructure;

public interface IAccountRepository
{
    Task AddAuthor(Author author);
}

public class AccountRepository : IAccountRepository
{
    private readonly ChirpDBContext _dbContext;
    
    public AccountRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAuthor(Author author)
    {
        _dbContext.Add(author);
        await _dbContext.SaveChangesAsync();
    }
}