using Chirp.Core;
using Chirp.Test.Stub;

namespace Chirp.Test;
using Xunit;
using Infrastructure;

public class CheepServiceUnitTests
{
    [Fact]
    public void GetCheeps_ReturnsCheeps()
    {
        // Arrange
        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService cheepService = new CheepService(cheepRepo);
        // Act
        List<CheepViewModel> cheeps = cheepService.GetCheeps(1);

        // Assert
        Assert.NotNull(cheeps);
        Assert.NotEmpty(cheeps);
    }
    
    [Fact] 
    public void GetCheepsFromAuthor_ReturnsCheeps()
    {
        // Arrange
        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService cheepService = new CheepService(cheepRepo);
        // Act
        List<CheepViewModel> cheeps = cheepService.GetCheepsFromAuthor(1, "Author1");

        // Assert
        Assert.NotNull(cheeps);
        Assert.NotEmpty(cheeps);
    }
    
    [Fact]
    public void TimestampToCEST_ConvertsCorrectly()
    {
        // Arrange
        var expectedTime = "18/10/2021 16:38:10";

        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService service = new CheepService(cheepRepo);

        // Act
        var result = service.GetCheeps(1);

        // Assert
        Assert.Equal(expectedTime, result[0].TimeStamp);
    }
    
    [Fact]
    public void GetAuthorByName_ReturnsAuthor()
    {
        // Arrange
        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService cheepService = new CheepService(cheepRepo);
        // Act
        Author author = cheepService.GetAuthorByName("Author1");

        // Assert
        Assert.NotNull(author);
        Assert.Equal("Author1", author.UserName);
    }
    
    [Fact]
    public void GetAuthorByEmail_ReturnsAuthor()
    {
        // Arrange
        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService cheepService = new CheepService(cheepRepo);
        // Act
        Author author = cheepService.GetAuthorByEmail("au1@itu.dk");
        
        // Assert
        Assert.NotNull(author);
        Assert.Equal("Author1", author.UserName);
    }
}