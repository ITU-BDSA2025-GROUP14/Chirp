using System.Collections;

using (var reader = new StreamReader(@"C:\Users\peter\Documents\ITU\3. Semester\BDSA\Chirp.CLI\data\chirp_cli_db.csv"))
{
    var chirps = new ArrayList();
    
    while (!reader.EndOfStream)
    {
        chirps.Add(reader.ReadLine());
    }

    foreach (var chirp in chirps)
    {
        var input = chirp.ToString().Split(',');
        Console.WriteLine(chirp);
    }
}