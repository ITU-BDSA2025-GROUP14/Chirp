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
    private static IDatabaseRepository<Cheep> database = new HttpDatabaseRepository();
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
        var cheeps = database.Read(int.MaxValue);
        UserInterface.PrintCheeps(cheeps);
        return 0;
    }

    static int RunCheep(string message)
    {
        var cheep = new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        database.Store(cheep);
        return RunRead();
    }
}