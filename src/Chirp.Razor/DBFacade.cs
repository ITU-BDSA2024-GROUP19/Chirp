using System.Data;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace Chirp.Razor
{
    public class DBFacade
    {
        public static List<CheepViewModel> Read()
        {
            List<CheepViewModel> cheeps = new List<CheepViewModel>();
            
            // Initialize SQLite provider ChatGPT help
            Batteries.Init();

            string sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
            if (sqlDBFilePath == null)
            {
                sqlDBFilePath = "/tmp/chirp.db";
            }
            
            string sqlQuery = @"SELECT u.username, m.text, m.pub_date FROM user u, message m WHERE u.user_id = m.author_id ORDER by m.pub_date desc";

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
                        cheeps.Add(new CheepViewModel($"{values[i]}", $"{values[i+1]}", $"{UnixTimeStampToDateTimeString(Convert.ToDouble(values[i+2]))}"));
                } 
            }
            return cheeps;
        }
        
        private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }
    }
}