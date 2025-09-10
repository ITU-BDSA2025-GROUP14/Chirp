using System.Collections;
using System.Globalization;
using CsvHelper.Configuration;

namespace SimpleDB;
using CsvHelper;

public class CSVDatabase<T>: IDatabaseRepository<T>
{
    private readonly string _filePath;

    public CSVDatabase(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath); 
        _filePath = filePath;
        
    }
    public IEnumerable<T> Read(int limit)
    {
        using var reader = new StreamReader(_filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<CheepMapper>();
        var records = csv.GetRecords<T>();
        return records.ToList();
    }
    
    public void Store(T record)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false, };
        using var writer = new StreamWriter(_filePath);
        using var csv = new CsvWriter(writer, config);
        
        if (!File.Exists(_filePath))
        {
                writer.WriteLine($"Author,Message,Timestamp");
        }

        csv.WriteRecord(record);
    }
}