namespace Chirp.Razor;
using System.Data;
using Microsoft.Data.Sqlite;

public class DBFacade
{
    private static DBFacade _instance;
    private string CHIRPDBPATH = Path.Combine(Path.GetTempPath(), "chirp.db");
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

        using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={CHIRPDBPATH}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = message;
            
        }
    }

    public List<CheepViewModel> getTimeline()
    {
        var cheeps = new List<CheepViewModel>();
        string sqlQuery = @"select user.username, message.text, message.pub_date from message message left outer join user user on author_id = user_id order by pub_date desc";
        using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={CHIRPDBPATH}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
           
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                string author = reader.GetString(0);
                string message = reader.GetString(1);
                string date = reader.GetString(2);

                cheeps.Add(new CheepViewModel(author, message, date));
            }
        }

        return cheeps;
    }
}