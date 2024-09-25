using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DBFacade
{
    public void DBConnection()
    {
        const string sqlDBFilePath = "data/chirp.db";
        const string sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";

        using var connection = new SqliteConnection($"Data Source={sqlDBFilePath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = sqlQuery;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
                
        }
    }
    
    
}