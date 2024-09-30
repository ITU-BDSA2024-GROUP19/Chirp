using System.Globalization;

using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace Chirp.Razor
{
    public class DBFacade
    {
        private static int getoffset(int page)
        {
            return (page - 1) * 32;
        }
        private static List<CheepViewModel> getcheeps(string sqlQuery)
        {
            List<CheepViewModel> cheeps = new List<CheepViewModel>();
            // SQLite provider needs to be initialized before making any calls to SQLite
            // Call to SQLitePCL.Batteries.Init() initializes the necessary provider for SQLite before attempting to open a connection to the database.
            // Information retrieved from ChatGPT.
            Batteries.Init();

            string sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? throw new NullReferenceException("No ChirpDBPATH environment variable");
            
            using var connection = new SqliteConnection($"Data Source={sqlDBFilePath}");
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = sqlQuery;

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Object[] values = new Object[reader.FieldCount];
                    int fieldCount = reader.GetValues(values);
                    for (int i = 0; i < fieldCount; i += 3)
                        cheeps.Add(new CheepViewModel($"{values[i]}", $"{values[i+1]}", $"{UnixTimeStampToDateTimeString(Convert.ToInt64(values[i+2]))}"));
                } 
            }
            return cheeps;
        }
        public static List<CheepViewModel> Read(int page)
        {
            int offset = getoffset(page);
            return getcheeps(@"SELECT u.username, m.text, m.pub_date FROM user u, message m WHERE u.user_id = m.author_id ORDER by m.pub_date desc LIMIT 32 OFFSET " + offset);
        }
        public static List<CheepViewModel> UserRead(string author, int page)
        {
            author = $"'{author}'";
            int offset = getoffset(page);
            return getcheeps(
                $"SELECT u.username, m.text, m.pub_date FROM user u, message m WHERE u.user_id = m.author_id AND u.username = {author} ORDER by m.pub_date desc LIMIT 32 OFFSET " + offset);
        }
        
        public static string UnixTimeStampToDateTimeString(long unixTimeStamp)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
            TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}