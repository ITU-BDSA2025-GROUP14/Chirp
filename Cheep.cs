using CsvHelper.Configuration.Attributes;

namespace Chirp.CLI;

public record Cheep(
    [Name("Author")] string Author, 
    [Name("Message")] string Message, 
    [Name("Timestamp")] long Timestamp
);