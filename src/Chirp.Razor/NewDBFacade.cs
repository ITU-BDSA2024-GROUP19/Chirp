using System.Globalization;

using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace Chirp.Razor;

class NewDBFacade : ICheepService
{
    public List<CheepViewModel> GetCheeps(int page)
    {
        throw new NotImplementedException();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        throw new NotImplementedException();
    }

    public static string UnixTimeStampToDateTimeString(long unixTimeStamp)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
        TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
        return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
    }
}