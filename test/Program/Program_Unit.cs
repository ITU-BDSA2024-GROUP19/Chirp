using Chirp.Program;

namespace Chirp.Test.Program;

public class Program_Unit
{
    [Fact]
    public void UnixTimeConversionTest()
    {
        //Arrange
        Cheep cheep = new ("Jakob", "test message", 1726563255);

        //Act
        long timestamp = cheep.Timestamp;

        // Assert
        Assert.Equal("17/09/2024 10:54:15", Cheep.TimecodeToCEST(timestamp));
    }

    [Fact]
    public void CorrectCLIMessageTest()
    {
        //Arrange
        string cliMessage = @"Chirp CLI version.

        Usage:
            chirp read [<limit>]
            chirp webRead [<limit>]
            chirp cheep <message>
            chirp bootLocalHost
            chirp (-h | --help)
            chirp --version

        Options:
            -h --help    Show this screen.
            --version    Show version information.
        ";

        //Act
        string getCLImessage = UserInterface.GetCLIMessage();

        // Assert
        Assert.Equal(cliMessage, getCLImessage);
    }

    [Fact]
    public void FormatOfCheepTest()
    {
        // Arrange
        Cheep cheep = new ("Jakob", "test message", 1726563255);
        string timestamp = Cheep.TimecodeToCEST(cheep.Timestamp);
        string expectedCheep = "Jakob" + " @ " + timestamp + ": " + "test message";
        
        // Act
        string cheepToString = cheep.ToString();
        
        // Assert
        Assert.Equal(expectedCheep, cheepToString);
    }

    [Fact]
    public void NewCheepWithPresentTimestampAndUserTest()
    {
        // Arrange
        string message = "test message";
        string expectedAuthor = Environment.UserName;
        
        // Act
        Cheep cheep = Cheep.NewCheep(message);
        
        // Assert
        Assert.True(Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - cheep.Timestamp) < 1);
        Assert.Equal(message, cheep.Message);
        Assert.Equal(expectedAuthor, cheep.Author);
    }

    [Fact]
    public void PrintsExpectedOutputTest()
    {
        // Arrange
        List<Cheep> cheeps = new()
        {
            new Cheep("Jakob", "Jakob's test cheep", 1726563255)
        }; ;

        // Act
        using StringWriter sw = new ();
        Console.SetOut(sw);  
        UserInterface.PrintCheeps(cheeps); 

        // Assert
        string result = sw.ToString();
        string expectedOutput = "Jakob @ 17/09/2024 10:54:15: Jakob's test cheep" + sw.NewLine;
        Assert.Equal(expectedOutput, result);
    }
}

    

