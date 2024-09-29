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
}