using Chirp.Core;

namespace Chirp.Infrastructure;

public interface IChirpAccountService
{
    void AddAuthor(Author author);
}

public class ChirpAccountService : IChirpAccountService
{
    
    private readonly IChirpAccountRepository _repository;
    
    public ChirpAccountService(IChirpAccountRepository repository)
    {
        _repository = repository;
    }
    
    public void AddAuthor(Author author)
    {
        _repository.AddAuthor(author);
    }
}