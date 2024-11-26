using Chirp.Core;

using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Test.Stub;

public class AuthorRepositoryStub : IAuthorRepository
{
    private readonly List<Author> _authors = new();

    public Task<IdentityResult> AddAuthor(Author author, string? password = null)
    {
        _authors.Add(author);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task FollowAuthor(string followerName, string authorName)
    {
        return Task.CompletedTask;
    }
    public Task UnfollowAuthor(string followerName, string authorName)
    {
        return Task.CompletedTask;
    }
}