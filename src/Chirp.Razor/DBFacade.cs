namespace Chirp.Razor;
using System.Data;
using Microsoft.Data.Sqlite;

public class DBFacade
{
    private static DBFacade _instance;
    private string sqlDBFilePath = "data/chirp.db";

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
        using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={sqlDBFilePath}"))
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
        using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={sqlDBFilePath}"))
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
                    
                // https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldatareader?view=dotnet-plat-ext-8.0#examples
                //var dataRecord = (IDataRecord)reader;
                //for (int i = 0; i < dataRecord.FieldCount; i++)
                  //  Console.WriteLine($"{dataRecord.GetName(i)}: {dataRecord[i]}");

                // See https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldatareader.getvalues?view=dotnet-plat-ext-8.0
                // for documentation on how to retrieve complete columns from query results
                //Object[] values = new Object[reader.FieldCount];
                //int fieldCount = reader.GetValues(values);
                //for (int i = 0; i < fieldCount; i++)
                  //  Console.WriteLine($"{reader.GetName(i)}: {values[i]}");
            }
        }
        return cheeps;
    }
}