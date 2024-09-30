using Chirp.Razor;
using Xunit;
using Xunit.Abstractions;

namespace Chirp.Razor.Tests;

public class TestDBFixture : IDisposable
{
    private string? originalDbPath;
    private string dbPath;
    
    public TestDBFixture()
    {
        originalDbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        dbPath = DBFacadeForTests.setupDBforTests();
    }

    public void Dispose()
    {
        DBFacadeForTests.removeDBforTests(originalDbPath, dbPath);
    }
}

public class ChirpRazorUnit
{
    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public void ReadSpecifiedAuthorsCheepsTest(string author)
    {
        //Arrange
        int expectedNumberOfCheeps = 1;
        
        //Act
        List<CheepViewModel> cheepsFromAuthor = DBFacade.UserRead(author, 1);
        
        //Assert
        Assert.Equal(expectedNumberOfCheeps, cheepsFromAuthor.Count);
    }

    [Fact]
    public void ReadAllCheepsOnPage1Test()
    {
        //Arrange
        int expectedNumberOfCheeps = 2;
        
        //Act
        List<CheepViewModel> cheeps = DBFacade.Read(1);
        
        //Assert
        Assert.Equal(expectedNumberOfCheeps, cheeps.Count);
    }
    
    [Fact]
    public void ReadAllCheepsOnPageAfterLastTest()
    {
        //Arrange
        int expectedNumberOfCheeps = 0;
        
        //Act
        List<CheepViewModel> cheeps = DBFacade.Read(2);
        
        //Assert
        Assert.Equal(expectedNumberOfCheeps, cheeps.Count);
    }

    [Fact]
    public void UnixTimeConversionTest()
    {
        //Arrange
        string expectedUnixTime = "17/09/2024 10:54:15";
        CheepViewModel cheep = new CheepViewModel("Jakob", "test message", "1726563255");
        
        //Act
        string returnedTime = DBFacade.UnixTimeStampToDateTimeString(Convert.ToInt64(cheep.Timestamp));
        
        //Assert
        Assert.Equal(expectedUnixTime, returnedTime);
    }

    [Fact]
    public void FormatOfCheepsTest()
    {
        //Arrange
        string expectedFormat1 = "Adrian Hej, velkommen til kurset. 01/08/2023 15:08:28";
        string expectedFormat2 = "Helge Hello, BDSA students! 01/08/2023 14:16:48";

        //Act
        List<CheepViewModel> cheeps = DBFacade.Read(1);
        string cheepFormat1 = cheeps[0].Author + " " + cheeps[0].Message + " " + cheeps[0].Timestamp;
        string cheepFormat2 = cheeps[1].Author + " " + cheeps[1].Message + " " + cheeps[1].Timestamp;

        //Assert
        Assert.Equal(expectedFormat1, cheepFormat1);
        Assert.Equal(expectedFormat2, cheepFormat2);
    }
}