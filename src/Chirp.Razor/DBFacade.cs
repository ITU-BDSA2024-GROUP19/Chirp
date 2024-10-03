using System.Globalization;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DBFacade : ICheepService, IDisposable
{
    private const int CHEEPS_PER_PAGE = 32;
    private readonly SqliteConnection Connection;

    public DBFacade()
    {
        string sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? CreateTemporaryDatabase();

        Connection = new SqliteConnection($"Data Source={sqlDBFilePath}");

        Connection.Open();

        string schema = "wwwroot/sample/schema.sql";
        string dump = "wwwroot/sample/dump.sql";

        if (File.Exists(schema) && File.Exists(dump))
        {
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = File.OpenText(schema).ReadToEnd();
            cmd.ExecuteNonQuery();
            cmd.CommandText = File.OpenText(dump).ReadToEnd();
            cmd.ExecuteNonQuery();
        }
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

    private string CreateTemporaryDatabase()
    {
        return Path.GetTempPath() + "chirp.db";
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