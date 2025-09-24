using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Chirp.WebService;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private readonly string _filePath;
    private static CSVDatabase<T>? _instance;

    private CSVDatabase()
    {
        _filePath = "../../data/chirp_cli_db.csv";
    }

    /// <summary>
    /// singleton pattern handling only one instanse of the CSVDatabase is active at once ...
    /// </summary>
    /// <returns></returns>
    public static CSVDatabase<T> GetInstance()
    {
        if (_instance == null)
        {
            _instance = new CSVDatabase<T>();
        }
        return _instance;
    }

    public IEnumerable<T> Read(int limit)
    {
        if (!File.Exists(_filePath))
            return Enumerable.Empty<T>();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };

        using var reader = new StreamReader(_filePath);
        using var csv = new CsvReader(reader, config);

        var records = new List<T>();
        csv.Read();
        csv.ReadHeader();

        int count = 0;
        while (csv.Read() && count < limit)
        {
            if (typeof(T) == typeof(Cheep))
            {
                var author = csv.GetField("Author") ?? "";
                var message = csv.GetField("Message") ?? "";
                var timestamp = csv.GetField<long>("Timestamp");
                var cheep = new Cheep(author, message, timestamp);
                records.Add((T)(object)cheep);
            }
            count++;
        }

        return records;
    }

    public void Store(T record)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false };
        var records = new List<T> { record };

        if (!File.Exists(_filePath))
        {
            using var headerWriter = new StreamWriter(_filePath);
            headerWriter.WriteLine("Author,Message,Timestamp");
        }

        using var stream = File.Open(_filePath, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, config);

        csv.WriteRecords(records);
    }
}