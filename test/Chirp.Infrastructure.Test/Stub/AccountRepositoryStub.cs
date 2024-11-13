using Chirp.Core;

namespace Chirp.Infrastructure.Test.Stub;

public class AccountRepositoryStub : IAccountRepository
{
    private readonly List<Author> _authors = new();

    public Task AddAuthor(Author author)
    {
        _authors.Add(author);
        return Task.CompletedTask;
    }
}