using Chirp.Core;

namespace Chirp.Infrastructure;

public interface IAccountService
{
    void AddAuthor(Author author);
}

public class AccountService : IAccountService
{
    
    private readonly IAccountRepository _repository;
    
    public AccountService(IAccountRepository repository)
    {
        _repository = repository;
    }
    
    public void AddAuthor(Author author)
    {
        _repository.AddAuthor(author);
    }
}