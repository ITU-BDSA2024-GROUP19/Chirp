using Chirp.SimpleDB;

using Microsoft.VisualStudio.TestPlatform.Utilities;

using Xunit.Abstractions;

namespace Chirp.Test.SimpleDB;

public class CSVDatabase_Integration : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly string _tempFilePath;

    public CSVDatabase_Integration(ITestOutputHelper output)
    {
        _output = output;
        _tempFilePath = _tempFilePath = Path.GetTempFileName();
    }

    public void Dispose()
    {
        if (File.Exists(_tempFilePath)) File.Delete(_tempFilePath);
    }

    [Fact]
    public void CSVDatabase_WriteTo_ReadFrom() {
        //Arrange:
        CreateSampleFile();
        List<Cheep> sample = new()
        {
            new("Alice", "Red Fox", 10000),
            new("Bob", "Brown Fox", 20000),
            new("Charlie", "Yellow Hen", 30000),
            new("Dorothy", "Green Snake", 40000)
        };

        //Act:
        IDatabaseRepository<Cheep> db = new CSVDatabase<Cheep>(_tempFilePath);
        sample.ForEach(db.Store);
        IEnumerable<Cheep> readAll = db.Read(0);
        IEnumerable<Cheep> readTwo = db.Read(2);
        IEnumerable<Cheep> readFour = db.Read(4);

        //Assert:
        Assert.Equal(sample, readAll);
        Assert.Equal(sample.GetRange(0, 2), readTwo);
        Assert.Equal(sample, readFour);
    }

        private void CreateSampleFile()
    {
        using var sw = File.CreateText(_tempFilePath);
        sw.Write("Author,Message,Timestamp");
        _output.WriteLine("Sample file created at: " + _tempFilePath);
    }
}

record Cheep(string Author, string Message, long Timestamp);