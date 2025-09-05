using System.Collections;
using Microsoft.VisualBasic.FileIO;
using Chirp.CLI.Client;

public class Program
{
    public static void Main(string[] args)
    {
        var path = "data/chirp_cli_db.csv";
        if (args.Length > 1)
        {
             if (args[0].ToLower().Equals("cheep"))
             {
                Cheep(path, args[1]);
             }
        }
        else
        {
            var cheeps = Read(path);
            UserInterface.PrintCheeps(cheeps);
        }
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