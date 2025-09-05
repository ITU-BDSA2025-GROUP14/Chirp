using CsvHelper.Configuration;

namespace Chirp.CLI;

public class CheepMap : ClassMap<Cheep>
{
    public CheepMap()
    {
        Map(m => m.author).Index(0).Name("author");
        Map(m => m.message).Index(1).Name("message");
        Map(m => m.timestamp).Index(2).Name("timestamp");
    }
}