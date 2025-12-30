namespace Chirp.Core;

public class Like
{
    public int LikeId { get; set; }
    public int CheepId { get; set; }
    public int AuthorId { get; set; }
    public DateTime TimeStamp { get; set; }

    public Cheep Cheep { get; set; } = null!;
    public Author Author { get; set; } = null!;
}
