using System.Diagnostics;

public class End2End
{
    [Fact]
    public void TestReadCheeps()
    {
        // Arrange
        ArrangeTestDatabase();
        // Act
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "./bin/Debug/net7.0/Chirp.dll read 10";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../src/Program/";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }
        string[] expectedLines = new string[]
        {
            "ropf @ 08/01/2023 14:09:20: Hello, BDSA students!",
            "adho @ 08/02/2023 14:19:38: Welcome to the course!",
            "adho @ 08/02/2023 14:37:38: I hope you had a good summer.",
            "ropf @ 08/02/2023 15:04:47: Cheeping cheeps on Chirp :)"
        };

        string[] outputLines = output.Split('\n');

        for (int i = 0; i < expectedLines.Length; i++)
        {
            Assert.Equal(expectedLines[i], outputLines[i].Trim());
        }
    }
    private void ArrangeTestDatabase()
    {
        /*
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "./src/Chirp.CLI.Client/bin/Debug/net7.0/Chirp.dll write ropf Hello, World!";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../";
            process.Start();
            process.WaitForExit();
        }
        */
    }
}