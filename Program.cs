using System.Collections;
using Microsoft.VisualBasic.FileIO;

using (var parser = new TextFieldParser(@"C:\Users\peter\Documents\ITU\3. Semester\BDSA\Chirp.CLI\data\chirp_cli_db.csv"))
{
    var chirps = new ArrayList();
    parser.TextFieldType = FieldType.Delimited;
    parser.SetDelimiters(",");
    
    while (!parser.EndOfData)
    {
        string[] fields = parser.ReadFields();
        chirps.Add(fields);
    }
    chirps.RemoveAt(0);

    foreach (string[] chirp in chirps)
    {
        var author      = chirp[0];
        var message   = chirp[1];
        var timestamp  = DateTimeOffset.FromUnixTimeSeconds(long.Parse(chirp[2]));
        Console.WriteLine($"{author} @ {timestamp.LocalDateTime}: {message}");
    }
}