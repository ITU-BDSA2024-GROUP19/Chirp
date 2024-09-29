using Chirp.Razor;

namespace Chirp.Razor.Tests;

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
}