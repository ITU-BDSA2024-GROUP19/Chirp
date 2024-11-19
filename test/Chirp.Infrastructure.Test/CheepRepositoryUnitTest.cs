using System.ComponentModel.DataAnnotations;

using Chirp.Core;
using Chirp.Infrastructure;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Chirp.Infrastructure.Test;

public class CheepRepositoryUnitTest : IAsyncLifetime
{
    private ChirpDBContext _context = null!;
    private ICheepRepository _cheepRepo = null!;

    private void DbTestInitializer()
    {
        var authors = new List<Author>
        {
            new() {UserName = "Author1", Email = "au1@itu.dk", Cheeps = new List<Cheep>(), Following = new List<Author>(), Followers = new List<Author>() }
        };
        var cheeps = new List<Cheep>
        {
            new() { CheepId = 1, Author = authors[0], Text = "Message 1", TimeStamp = DateTime.UtcNow },
            new() { CheepId = 2, Author = authors[0], Text = "Message 2", TimeStamp = DateTime.UtcNow.AddSeconds(5) },
            new() { CheepId = 3, Author = authors[0], Text = "Message 3", TimeStamp = DateTime.UtcNow.AddSeconds(10) }
        };
        _context.Authors.AddRange(authors);
        _context.Cheeps.AddRange(cheeps);
        _context.SaveChanges();
    }
    
    public async Task InitializeAsync()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        _context = new ChirpDBContext(builder.Options);
        await _context.Database.EnsureCreatedAsync();
        
        _cheepRepo = new CheepRepository(_context);
    }
    
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
    [Fact]
    public async Task AddCheep_SavesCheepToDatabase()
    {
        // Arrange
        var a13 = new Author() { UserName = "Test Author", Email = "test@itu.dk", Cheeps = new List<Cheep>(), Following = new List<Author>(), Followers = new List<Author>() };
        var cheep = new Cheep() { CheepId = 658, Author = a13, Text = "Test Message", TimeStamp = DateTime.UtcNow };
        
        // Act
        await _cheepRepo.AddCheep(cheep);

        // Assert
        var storedCheep = await _context.Cheeps.Include(c => c.Author).FirstOrDefaultAsync();
        Assert.NotNull(storedCheep);
        Assert.Equal("Test Author", storedCheep.Author.UserName);
        Assert.Equal("Test Message", storedCheep.Text);
    }
    [Fact]
    public async Task AddAuthor_SavesAuthorToDatabase()
    {
        // Arrange
        var author = new Author() { UserName = "Test Author", Email = "test@itu.dk", Cheeps = new List<Cheep>(), Following = new List<Author>(), Followers = new List<Author>() };

        // Act
        await _cheepRepo.AddAuthor(author);

        // Assert
        var storedAuthor = await _context.Authors.FirstOrDefaultAsync();
        Assert.NotNull(storedAuthor);
        Assert.Equal("Test Author", storedAuthor.UserName);
    }

    [Fact]
    public async Task GetAuthorByName_ReturnsAuthor()
    {
        // Arrange
        var author = new Author()
        {
            UserName = "Test Author", Email = "test@itu.dk", Cheeps = new List<Cheep>(), Following = new List<Author>(), Followers = new List<Author>()
        };

        // Act
        await _cheepRepo.AddAuthor(author);

        // Assert
        var storedAuthor = await _cheepRepo.GetAuthorByName("Test Author");
        Assert.NotNull(storedAuthor);
    }
    
    [Fact]
    public async Task GetAuthorByName_WithUnknownAuthor_ReturnsNull()
    {
        // Arrange
        var author = new Author()
        {
            UserName = "Test Author", Email = "test@itu.dk", Cheeps = new List<Cheep>(), Following = new List<Author>(), Followers = new List<Author>()
        };
        
        // Act
        await _cheepRepo.AddAuthor(author);
        
        // Assert
        var storedAuthor = await _cheepRepo.GetAuthorByName("Unknown Author");
        Assert.Null(storedAuthor);
    }
    
    [Fact]
    public async Task GetAuthorByEmail_ReturnsAuthor()
    {
        // Arrange
        var author = new Author()
        {
            UserName = "Test Author", Email = "test@itu.dk", Cheeps = new List<Cheep>(), Following = new List<Author>(), Followers = new List<Author>()
        };
        
        // Act
        await _cheepRepo.AddAuthor(author);
        
        // Assert
        var storedAuthor = await _cheepRepo.GetAuthorByEmail("test@itu.dk");
        Assert.NotNull(storedAuthor);
    }
    
    [Fact]
    public async Task GetAuthorByEmail_WithUnknownEmail_ReturnsNull()
    {
        // Arrange
        var author = new Author()
        {
            UserName = "Test Author", Email = "test@itu.dk", Cheeps = new List<Cheep>(), Following = new List<Author>(), Followers = new List<Author>()
        };
        
        // Act
        await _cheepRepo.AddAuthor(author);
        
        // Assert
        var storedAuthor = await _cheepRepo.GetAuthorByEmail("Unknown Email");
        Assert.Null(storedAuthor);
    }

    [Fact]
    public async Task GetCheepDTO_ReturnsPagedCheeps()
    {
        // Arrange
        DbTestInitializer();
        
        // Act
        var cheeps = await _cheepRepo.GetCheepDTO(1, "Author1");

        // Assert
        Assert.NotEmpty(cheeps);
        Assert.Equal(3, cheeps.Count);
        Assert.Equal("Message 3", cheeps[0].Message);
    }

    [Fact]
    public async Task GetCheepDTO_WithNoCheepsOnPage_ReturnsEmptyList()
    {
        // Arrange
        DbTestInitializer();
        
        // Act
        var cheeps = await _cheepRepo.GetCheepDTO(2,"UnknownAuthor");

        // Assert
        Assert.Empty(cheeps);
    }

    [Fact]
    public async Task GetCheepDTOFromAuthor_ReturnsCheepsByAuthor()
    {
        // Arrange
        DbTestInitializer();
        
        // Act
        var cheepsByAuthor = await _cheepRepo.GetCheepDTOFromAuthor(1, "Author1", "Author1");

        // Assert
        Assert.NotEmpty(cheepsByAuthor);
        Assert.All(cheepsByAuthor, cheep => Assert.Equal("Author1", cheep.Author));
    }
    
    [Fact]
    public async Task GetCheepDTOFromAuthor_WithNoCheepsForAuthor_ReturnsEmptyList()
    {
        // Arrange
        DbTestInitializer();
        // Act
        var cheepsByUnknownAuthor = await _cheepRepo.GetCheepDTOFromAuthor(1, "UnknownAuthor", "UnknownAuthor");

        // Assert
        Assert.Empty(cheepsByUnknownAuthor);
    }

    [Fact]
    public async Task AddCheepWithTooManyCharacters()
    {
        // Arrange
        DbTestInitializer();
        var a14 = new Author() { UserName = "Test Author", Email = "test@itu.dk", Cheeps = new List<Cheep>(), Following = new List<Author>(), Followers = new List<Author>() };
        var cheep = new Cheep() { CheepId = 658, Author = a14, Text = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH", TimeStamp = DateTime.UtcNow };
        
        // Act And Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationException>(() => _cheepRepo.AddCheep(cheep));
        Assert.Equal("Cheep content must be less than 160 characters!", exception.Message);
    }
}