using CsvHelper.Configuration.Attributes;

namespace Chirp.CLI;

public class Cheep
{
    [Name("author")]
    public string author { get; set; }
    [Name("message")]
    public string message { get; set; }
    [Name("timestamp")]
    public long timestamp { get; set; }
}