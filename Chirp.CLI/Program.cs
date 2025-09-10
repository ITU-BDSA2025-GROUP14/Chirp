using System.Collections;
using System.Globalization;
using SimpleDB;

using Chirp.CLI;

using Microsoft.VisualBasic.FileIO;
using Chirp.CLI.Client;
using CommandLine;

    
using CsvHelper;
using CsvHelper.Configuration;

using Cheep = SimpleDB.Cheep;

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
    private static String _filePath = "data/chirp_cli_db.csv";
    private static CSVDatabase<Cheep> db = new CSVDatabase<Cheep>(_filePath);
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
        var records = db.Read(0);
        UserInterface.PrintCheeps(records);
        return 0;
    }

    static int RunCheep(string message)
    {
        RunRead();
        Cheep(message);
        return 0;
    }
    private static void Cheep(string message)
    {
        db.Store(new Cheep(Environment.MachineName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
    }
}