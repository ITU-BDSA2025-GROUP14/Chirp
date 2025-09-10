using CsvHelper.Configuration;

namespace Chirp.CLI;

public class CheepMap : ClassMap<Cheep>
{
    public CheepMap()
    {
        Map(m => m.Author).Index(0).Name("Author");
        Map(m => m.Message).Index(1).Name("Message");
        Map(m => m.Timestamp).Index(2).Name("Timestamp");
    }
}