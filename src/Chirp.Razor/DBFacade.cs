namespace Chirp.Razor;
using System.Data;
using Microsoft.Data.Sqlite;

public class DBFacade
{
    private static DBFacade _instance;
    
    public static string GetConnectionString()
    {
        string envVarPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        if (!String.IsNullOrEmpty(envVarPath))
        {
            return envVarPath;
        }
        
        string tempDir = Path.GetTempPath();
        return Path.Combine(tempDir, "chirp.db");
    }
    
    private DBFacade()
    {
    }

    public static DBFacade GetInstance()
    {
        if (_instance == null)
        {
            _instance = new DBFacade();
        }
        return _instance;
    }

    public void SaveMessage(string message)
    {
        using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={GetConnectionString()}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = message;
            command.ExecuteNonQuery();
        }
    }

    public List<CheepViewModel> GetTimeline()
    {
        var cheeps = new List<CheepViewModel>();
        string sqlQuery = @"select user.username, message.text, message.pub_date from message message left outer join user user on author_id = user_id order by pub_date desc";
        using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={GetConnectionString()}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {

                string author = reader.GetString(0);
                string message = reader.GetString(1);
                double date = reader.GetDouble(2);


                cheeps.Add(new CheepViewModel(author, message, UnixTimeStampToDateTimeString(date)));
            }
        }
        return cheeps;
    }

    public List<CheepViewModel> GetTimelineByAuthor(string author)
    {
        var cheeps = new List<CheepViewModel>();
        string sqlQuery = @"select user.username, message.text, message.pub_date from message message left outer join user user on author_id = user_id where user.username = $author order by pub_date desc";
        using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={GetConnectionString()}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            command.Parameters.AddWithValue("$author", author);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {

                string authorName = reader.GetString(0);
                string message = reader.GetString(1);
                double date = reader.GetDouble(2);


                cheeps.Add(new CheepViewModel(authorName, message, UnixTimeStampToDateTimeString(date)));
            }
        }
        return cheeps;
    }
    
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}