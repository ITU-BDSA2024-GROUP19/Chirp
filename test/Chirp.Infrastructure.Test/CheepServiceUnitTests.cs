using Chirp.Infrastructure.Test.Stub;

namespace Chirp.Test;
using Chirp.Infrastructure.Cheeps;

using Xunit;

public class CheepServiceUnitTests
{
    [Fact]
    public void GetCheeps_ReturnsCheeps()
    {
        // Arrange
        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService cheepService = new CheepService(cheepRepo, null!);
        // Act
        List<CheepDto> cheeps = cheepService.GetCheeps(1, "Author1");

        // Assert
        Assert.NotNull(cheeps);
        Assert.NotEmpty(cheeps);
    }

    [Fact]
    public void GetCheepsFromAuthor_ReturnsCheeps()
    {
        // Arrange
        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService cheepService = new CheepService(cheepRepo, null!);
        // Act
        List<CheepDto> cheeps = cheepService.GetCheepsFromAuthor(1, "Author1", "Author1");

        // Assert
        Assert.NotNull(cheeps);
        Assert.NotEmpty(cheeps);
    }

    [Fact]
    public void CheepService_CanRetrieveCorrectTimeFromCheep()
    {
        // Arrange
        var expectedTime = 1634567890;

        ICheepRepository cheepRepo = new CheepRepositoryStub();
        ICheepService service = new CheepService(cheepRepo, null!);

        // Act
        var result = service.GetCheeps(1, "Author1");

        // Assert
        Assert.Equal(expectedTime, result[0].Timestamp);
    }
}