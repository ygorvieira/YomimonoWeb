namespace Yomimono.Domain.Entities;

public class BookAuthor
{
    public Guid BookId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Role { get; private set; } = "Author";
    public Book Book { get; private set; } = null!;
    public Author Author { get; private set; } = null!;

    private BookAuthor() { }

    public BookAuthor(Guid bookId, Guid authorId, string role = "Author")
    {
        BookId = bookId;
        AuthorId = authorId;
        Role = role;
    }
}
