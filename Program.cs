using System.Collections;
using Microsoft.VisualBasic.FileIO;
using Chirp.CLI.Client;
using CommandLine;

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
        return Parser.Default.ParseArguments<ReadOptions, CheepOptions>(args)
            .MapResult(
                (ReadOptions opts) => RunRead(),
                (CheepOptions opts) => RunCheep(opts.Message),
                errs => 1);
    }

    static int RunRead()
    {
        var path = "data/chirp_cli_db.csv";
        var cheeps = Read(path);
        UserInterface.PrintCheeps(cheeps);
        return 0;
    }

    static int RunCheep(string message)
    {
        var path = "data/chirp_cli_db.csv";
        Cheep(path, message);
        return 0;
    }

    public static IEnumerable<Cheep> Read(string path)
    {
        using (var parser = new TextFieldParser(path))
        {
            var chirps = new ArrayList();
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
    
            while (!parser.EndOfData)
            {
                string[]? fields = parser.ReadFields();
                if (fields != null)
                {
                    chirps.Add(fields);
                }
            }
            chirps.RemoveAt(0);

            var cheeps = new List<Cheep>();
            foreach (string[] chirp in chirps)
            {
                var author = chirp[0];
                var message = chirp[1];
                var timestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(chirp[2]));
                cheeps.Add(new Cheep(author, message, timestamp));
            }
            
            return cheeps;
        }
    }
    public static void Cheep(string path, string message)
    {
        if (!File.Exists(path))
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine($"Author,Message,Timestamp");
            }
        }

        using (var writer = File.AppendText(path))
        {
            writer.WriteLine($"{Environment.UserName},\"{message}\",{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}");
        }
    }
}