using Xunit;

namespace Chirp.Test.Program;

public class Program_Unit
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