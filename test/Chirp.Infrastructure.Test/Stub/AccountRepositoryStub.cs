using Chirp.Core;

using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Test.Stub;

public class ChirpAccountRepositoryStub : IAuthorRepository
{
    private readonly List<Author> _authors = new();

    public Task<IdentityResult> AddAuthor(Author author, string? password = null)
    {
        _authors.Add(author);
        return Task.FromResult(IdentityResult.Success);
    }
}