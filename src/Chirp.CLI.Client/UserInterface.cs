using SimpleDB;

namespace Chirp.CLI.Client;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            var timestamp = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp);
            Console.WriteLine($"{cheep.Author} @ {timestamp.LocalDateTime}: {cheep.Message}");
        }
    }
}
