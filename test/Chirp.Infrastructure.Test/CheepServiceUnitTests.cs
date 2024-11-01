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
    
    /*
    [Fact]
    public void GetCheeps_WithValidPage_ReturnsCheeps()
    {
        // Arrange
        var mockRepository = new Mock<ICheepRepository>();
        mockRepository.Setup(repo => repo.GetCheepDTO(1))
            .ReturnsAsync(new List<CheepDTO>
            {
                new CheepDTO("Author1", "Message 1", 1634567890),
                new CheepDTO("Author2", "Message 2", 1634567900)
            });

        var service = new CheepService(mockRepository.Object);

        // Act
        var result = service.GetCheeps(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Author1", result[0].Author);
        Assert.Equal("Message 1", result[0].Message);
        Assert.Equal("18/10/2021 16:38:10", result[0].TimeStamp);
    }
    
    [Fact]
    public void GetCheeps_WithNoCheeps_ReturnsEmptyList()
    {
        // Arrange
        var mockRepository = new Mock<ICheepRepository>();
        mockRepository.Setup(repo => repo.GetCheepDTO(2)).ReturnsAsync(new List<CheepDTO>());

        var service = new CheepService(mockRepository.Object);

        // Act
        var result = service.GetCheeps(2);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public void GetCheepsFromAuthor_WithValidAuthor_ReturnsCheeps()
    {
        // Arrange
        var mockRepository = new Mock<ICheepRepository>();
        mockRepository.Setup(repo => repo.GetCheepDTOFromAuthor(1, "Author1"))
            .ReturnsAsync(new List<CheepDTO>
            {
                new CheepDTO("Author1", "Message 1", 1634567890)
            });

        var service = new CheepService(mockRepository.Object);

        // Act
        var result = service.GetCheepsFromAuthor(1, "Author1");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Author1", result[0].Author);
        Assert.Equal("Message 1", result[0].Message);
        Assert.Equal("18/10/2021 16:38:10", result[0].TimeStamp);
    }
    
    [Fact]
    public void GetCheepsFromAuthor_WithNoCheepsFromAuthor_ReturnsEmptyList()
    {
        // Arrange
        var mockRepository = new Mock<ICheepRepository>();
        mockRepository.Setup(repo => repo.GetCheepDTOFromAuthor(1, "UnknownAuthor"))
            .ReturnsAsync(new List<CheepDTO>());

        var service = new CheepService(mockRepository.Object);

        // Act
        var result = service.GetCheepsFromAuthor(1, "UnknownAuthor");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public void TimestampToCEST_ConvertsCorrectly()
    {
        // Arrange
        var timestamp = 1634567890L;
        var expectedTime = "18/10/2021 16:38:10";

        var mockRepository = new Mock<ICheepRepository>();
        mockRepository.Setup(repo => repo.GetCheepDTO(1))
            .ReturnsAsync(new List<CheepDTO> { new CheepDTO("Author1", "Message", timestamp) });

        var service = new CheepService(mockRepository.Object);

        // Act
        var result = service.GetCheeps(1);

        // Assert
        Assert.Equal(expectedTime, result[0].TimeStamp);
    }
    
    [Fact]
    public void GetCheeps_WithRepositoryError_ThrowsException()
    {
        // Arrange
        var mockRepository = new Mock<ICheepRepository>();
        mockRepository.Setup(repo => repo.GetCheepDTO(1))
            .ThrowsAsync(new Exception("Database error"));

        var service = new CheepService(mockRepository.Object);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => Task.FromResult(service.GetCheeps(1)));
    }
    */
}