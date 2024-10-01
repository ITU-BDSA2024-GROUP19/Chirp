using System.Globalization;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace Chirp.Razor;

class NewDBFacade : ICheepService, IDisposable
{
    private const int CHEEPS_PER_PAGE = 32;
    private readonly SqliteConnection Connection;

    public NewDBFacade()
    {
        // Call to SQLitePCL.Batteries.Init() initializes the necessary provider for SQLite before attempting to open a connection to the database.
        // Information retrieved from ChatGPT.
        //Batteries.Init();
        string sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? Path.GetTempPath() + "chirp.db";
        Connection = new SqliteConnection($"Data Source={sqlDBFilePath}");
        Connection.Open();
    }
    
    public List<CheepViewModel> GetCheeps(int page)
    {
        List<CheepViewModel> cheeps = new List<CheepViewModel>();
        var command = Connection.CreateCommand();
        command.CommandText = "SELECT u.username, m.text, m.pub_date FROM user u, message m WHERE u.user_id = m.author_id ORDER by m.pub_date desc LIMIT " + CHEEPS_PER_PAGE + " OFFSET " + getPageOffset(page);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            Object[] values = new Object[reader.FieldCount];
            int fieldCount = reader.GetValues(values);
            for (int i = 0; i < fieldCount; i += 3)
                cheeps.Add(new CheepViewModel($"{values[i]}", $"{values[i+1]}", $"{UnixTimeStampToDateTimeString(Convert.ToInt64(values[i+2]))}"));
        }
        return cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        List<CheepViewModel> cheeps = new List<CheepViewModel>();
        var command = Connection.CreateCommand();
        command.CommandText = $"SELECT u.username, m.text, m.pub_date FROM user u, message m WHERE u.user_id = m.author_id AND u.username = '{author}' ORDER by m.pub_date desc LIMIT " + CHEEPS_PER_PAGE + " OFFSET " + getPageOffset(page);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            Object[] values = new Object[reader.FieldCount];
            int fieldCount = reader.GetValues(values);
            for (int i = 0; i < fieldCount; i += 3)
                cheeps.Add(new CheepViewModel($"{values[i]}", $"{values[i+1]}", $"{UnixTimeStampToDateTimeString(Convert.ToInt64(values[i+2]))}"));
        }
        return cheeps;
    }

    public void Dispose()
    {
        Connection.Dispose();
    }

    public static string UnixTimeStampToDateTimeString(long unixTimeStamp)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
        TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
        return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
    }

    private static int getPageOffset(int page)
    {
        return (page - 1) * CHEEPS_PER_PAGE;
    }
}