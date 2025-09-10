namespace SimpleDB;

public class Cheep
{
    //CsvHelper needs an empty constructor to read and populate Cheep class.
    public Cheep()
    {
        
    }
    public Cheep(string author, string message, long timestamp)
    {
        Author = author;
        Message = message;
        Timestamp = timestamp;
    }
    public string Author { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }
}