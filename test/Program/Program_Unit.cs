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
    public void ExampleTest2()
    {
        Assert.False(2 + 2 == 5, $"The state says two plus two should be five");
    }
}