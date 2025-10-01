using System.Data;

using Chirp.Razor;
using Microsoft.Data.Sqlite;




public record CheepViewModel(string Author, string Message, string Timestamp);

public class CheepService
{
    private DBFacade _connection = DBFacade.GetInstance();

    //These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new()
    { 
    };
   
    
    public List<CheepViewModel> GetCheeps()
    {
        _cheeps.AddRange(_connection.getTimeline());
        return _cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return _cheeps.Where(x => x.Author == author).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
    

}
