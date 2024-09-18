using System.Globalization;

namespace Chirp.Program;

public record Cheep(string Author, string Message, long Timestamp)
{
    /// <summary>
    /// Builder method for new Cheeps.
    /// </summary>
    /// <returns>A Cheep with the message content. Includes the current time, and env. username.</returns>
    public static Cheep NewCheep(string message) 
    {
        return new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    override public string ToString()
    {
        return Author + " @ " + TimecodeToCEST(Timestamp) + ": " + Message;
    }
    
    private static string TimecodeToCEST(long timecode)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timecode);
        TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
        return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture); // ensures the right format that is required
    }
}