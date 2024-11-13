using Chirp.Core;

namespace Chirp.Infrastructure;

public class AccountRepository
{
    public interface IAccountRepository
    {
        Task AddAuthor(Author author);
    }
}