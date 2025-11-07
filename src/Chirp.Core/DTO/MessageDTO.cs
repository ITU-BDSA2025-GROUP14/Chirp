namespace Chirp.Core.DTO;

public class MessageDto
{
    public string Author { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
