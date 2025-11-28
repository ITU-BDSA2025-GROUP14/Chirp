namespace Chirp.Core;

public class Author
{
    public int AuthorId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; }
    public List<Author> followings { get; set; }
    public List<Author> followers { get; set; }

}