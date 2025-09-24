
namespace Chirp.CLI.Client;

public static class UserInterface
{
    public static string FormatCheep(Cheep cheep)
    {
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).ToLocalTime();
        return $"{cheep.Author} @ {timestamp:yyyy-MM-dd HH:mm}: {cheep.Message}";
    }
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            Console.WriteLine(FormatCheep(cheep));   
        }
        
    }

}