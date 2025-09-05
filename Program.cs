using System.Collections;
using System.Globalization;
using System.Net.Mime;

using Chirp.CLI;

using CsvHelper;
using CsvHelper.Configuration;

using Microsoft.VisualBasic.FileIO;

public class Program
{
    public static void Main(string[] args)
    {
        var path = "data/chirp_cli_db.csv";
        Console.WriteLine();
        
        if (args.Length > 1)
        {
            if (args[0].ToLower().Equals("cheep"))
            {
                Cheep(path, args[1]);
            }
        }
    }

    static public void reader(string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, config))
        {   
            csv.Context.RegisterClassMap<CheepMap>();
            var records = csv.GetRecords<Cheep>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var timestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(csv.GetField("timestamp")));
                Console.WriteLine($"{csv.GetField("author")} @ {timestamp.LocalDateTime}: {csv.GetField("message")}");
            }
        }
    }

    static public void writer(string path, string message)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false, };
        var records = new List<Cheep>
        {
            new Cheep
            {
                author = Environment.UserName, message = message, timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }
        };
        using (var stream = File.Open(path, FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecords(records);
        }
    }
    public static void Cheep(string path, string message)
    {
        writer(path, message);
        reader(path);
    }
}