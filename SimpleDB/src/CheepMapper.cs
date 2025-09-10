using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CheepMapper : ClassMap<Cheep>
{
    public CheepMapper()
    {
        Map(m => m.Author).Index(0).Name("Author");
        Map(m => m.Message).Index(1).Name("Message");
        Map(m => m.Timestamp).Index(2).Name("Timestamp");
    }
}


