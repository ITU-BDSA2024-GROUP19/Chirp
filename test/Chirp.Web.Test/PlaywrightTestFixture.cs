using System.Diagnostics;

namespace Chirp.Web.Test;

public class PlaywrightTestFixture
{
    public static async Task<Process> StartServer()
    {
        Environment.SetEnvironmentVariable("CHIRPDBPATH", ":memory:");
        // Path to the project directory containing the Chirp.Web project
        var projectDirectory = @"../../../../../src/Chirp.Web"; // Update this path to your Chirp.Web project folder

        // Start the server process
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run", // Command to run the application
            WorkingDirectory = projectDirectory, // Set the working directory to the Chirp.Web project
            RedirectStandardOutput = true, // Optional: Capture output
            RedirectStandardError = true,
            UseShellExecute = false, // Ensures that the process is launched correctly
            CreateNoWindow = true // Run without opening a new window
        };

        var process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();

        // Optional: Wait for the server to start before proceeding (e.g., check the server log)
        await Task.Delay(6000); // Wait for the server to be fully initialized

        return process;
    }
}