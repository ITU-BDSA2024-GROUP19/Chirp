using Chirp.Program;

namespace Chirp.Test.Program;

public class Program_Unit
{
    [Fact]
    public void UnixTimeConversionTest()
    {
        //Arrange
        var cheep = new Cheep("Jakob", "test message", 1726563255);

        //Act
        long timestamp = cheep.Timestamp;

        // Assert
        Assert.Equal("17/09/2024 10:54:15", Cheep.TimecodeToCEST(timestamp));
    }

    [Fact]
    public void CorrectCLIMessageTest()
    {
        //Arrange
        string climessage = @"Chirp CLI version.

        Usage:
            chirp read [<limit>]
            chirp cheep <message>
            chirp (-h | --help)
            chirp --version

        Options:
            -h --help    Show this screen.
            --version    Show version information.
        ";

        //Act
        string getclimessage = UserInterface.GetCLIMessage();

        // Assert
        Assert.Equal(climessage, getclimessage);
    }

    [Fact]
    public void NewCheepTest()
    {
        // Arrange
        var newCheep = Cheep.NewCheep("test message");
        string cheepToString = newCheep.ToString();
        string timestamp = Cheep.TimecodeToCEST(newCheep.Timestamp);
        
        
        // Act
        string expectedCheep = Environment.UserName + " @ " + timestamp + ": " + "test message";
        
        // Assert
        Assert.Equal(expectedCheep, cheepToString);
    }

    [Fact]
    public void PrintCheepPrintsExpectedOutputTest()
    {
        // Arrange
        var cheeps = new List<Cheep>
        {
            new Cheep("Jakob", "Jakob's test cheep", 1726563255),  // Example Unix timestamps
            new Cheep("Ronas", "Ronas' test cheep", 1726563255),
            new Cheep("Jacob", "Jacob's test cheep", 1726563255)
        };
        
        string expectedOutput = 
            "Jakob @ 17/09/2024 10:54:15: Jakob's test cheep\n" +
            "Ronas @ 17/09/2024 10:54:15: Ronas' test cheep\n" +
            "Jacob @ 17/09/2024 10:54:15: Jacob's test cheep\n";

        // Act
        using var sw = new StringWriter();
        Console.SetOut(sw);  
        UserInterface.PrintCheeps(cheeps); 

        // Assert
        var result = sw.ToString();
        Assert.Equal(expectedOutput, result);
    }
}

    

