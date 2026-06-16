namespace Yomimono.Domain.Entities;

public class BookGenre
{
    public Guid BookId { get; private set; }
    public Guid GenreId { get; private set; }
    public Book Book { get; private set; } = null!;
    public Genre Genre { get; private set; } = null!;

    private BookGenre() { }

    public BookGenre(Guid bookId, Guid genreId)
    {
        BookId = bookId;
        GenreId = genreId;
    }
}
