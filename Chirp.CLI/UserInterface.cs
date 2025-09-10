namespace Chirp.CLI.Client;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<SimpleDB.Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            Console.WriteLine($"{cheep.Author} @ {cheep.Timestamp}: {cheep.Message}");
        }
    }
}
