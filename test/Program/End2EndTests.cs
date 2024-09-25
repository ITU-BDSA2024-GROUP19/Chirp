using System.Diagnostics;

public class End2End
{
    //These do not reflect the web API. These needs to be rewritten for web app
    [Fact]
    public void TestRead5FristsCheeps()
    {
        /*
        // Act
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "./bin/Debug/net7.0/Chirp.dll read 5";
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
            "ropf @ 01/08/2023 14:09:20: Hello, BDSA students!",
            "adho @ 02/08/2023 14:19:38: Welcome to the course!",
            "adho @ 02/08/2023 14:37:38: I hope you had a good summer.",
            "ropf @ 02/08/2023 15:04:47: Cheeping cheeps on Chirp :)"
        };

        string[] outputLines = output.Split('\n');

        for (int i = 0; i < expectedLines.Length; i++)
        {
            Assert.Equal(expectedLines[i], outputLines[i].Trim());
        }
        */
    }
    
    //These do not reflect the web API. These needs to be rewritten for web app
    [Fact]
    public void TestStoreCheep()
    {
        /*
        // Act
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "./bin/Debug/net7.0/Chirp.dll cheep \"Hello, world!\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../src/Program/";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }
        string expectedOutput = "Cheep posted successfully!" + Environment.NewLine;

        Assert.Equal(expectedOutput, output);
        */
    }
}