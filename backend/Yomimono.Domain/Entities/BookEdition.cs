namespace Yomimono.Domain.Entities;

public class BookEdition
{
    public Guid Id { get; private set; }
    public Guid BookId { get; private set; }
    public string Name { get; private set; } = null!;
    public int Number { get; private set; }
    public Book Book { get; private set; } = null!;

    private BookEdition() { }

    public BookEdition(Guid bookId, string name, int number)
    {
        Id = Guid.NewGuid();
        BookId = bookId;
        Name = name;
        Number = number;
    }
}
