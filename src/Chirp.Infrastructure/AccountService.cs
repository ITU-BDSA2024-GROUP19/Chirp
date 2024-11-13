using Chirp.Core;

namespace Chirp.Infrastructure;

public class AccountService
{
    public interface IAccountService
    {
        void AddAuthor(Author author);
    }
}