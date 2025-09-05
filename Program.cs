using System.Collections;
using System.Globalization;

using Chirp.CLI;

using Microsoft.VisualBasic.FileIO;
using Chirp.CLI.Client;
using CommandLine;

using CsvHelper;
using CsvHelper.Configuration;

// the options that are for the cheep command
[Verb("cheep", HelpText = "Create a new cheep")]
public class CheepOptions
{
    [Value(0, MetaName = "message", Required = true, HelpText = "The message to cheep")]
    public string Message { get; set; } = string.Empty;
}

// the options which are for the default read command (meaning no verb)
[Verb("read", isDefault: true, HelpText = "Read and display all cheeps")]
public class ReadOptions
{
    // no more additional options for read command needed
}

public class Program
{
    public static int Main(string[] args)
    {
        var path = "data/chirp_cli_db.csv";
        return Parser.Default.ParseArguments<ReadOptions, CheepOptions>(args)
            .MapResult(
                (ReadOptions opts) => RunRead(path),
                (CheepOptions opts) => RunCheep(path, opts.Message),
                errs => 1);
    }

    static int RunRead(string path)
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
            UserInterface.PrintCheeps(records);
        }
        return 0;
    }

    static int RunCheep(string path, string message)
    {
        RunRead(path);
        Cheep(path, message);
        return 0;
    }
    public static void Cheep(string path, string message)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false, };
        var records = new List<Cheep>
        {
            new Cheep
            {
                author = Environment.UserName, message = message, timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }
        };
        if (!File.Exists(path))
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine($"Author,Message,Timestamp");
            }
        }

        using (var stream = File.Open(path, FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecords(records);
        }
    }
}