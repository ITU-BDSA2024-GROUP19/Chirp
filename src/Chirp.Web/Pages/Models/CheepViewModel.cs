namespace Chirp.Web.Pages.Models;

public record CheepViewModel(
    int Id, 
    string Author, 
    string Message, 
    string TimeStamp, 
    bool IsFollowed, 
    int LikeCount, 
    bool IsLikedByUser) 
{
    public static string TimestampToCEST(long timestamp)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
        return dateTime.ToString("dd/MM/yyyy HH:mm:ss"); // ensures the right format that is required
    }
}
