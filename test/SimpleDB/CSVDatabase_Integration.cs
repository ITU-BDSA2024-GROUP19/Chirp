using Xunit;

namespace Chirp.Test.SimpleDB;

public class CSVDatabase_Integration
{
    [Fact]
    public void ExampleTest1()
    {
        Assert.True(2 + 2 == 4, $"Two plus two should be four");
    }

    [Fact]
    public void ExampleTest2()
    {
        Assert.False(2 + 2 == 5, $"The state says two plus two should be five");
    }
}