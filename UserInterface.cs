namespace Chirp.CLI.Client;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            Console.WriteLine($"{cheep.Author} @ {cheep.Timestamp.LocalDateTime}: {cheep.Message}");
        }
    }
}

public record Cheep(string Author, string Message, DateTimeOffset Timestamp);
