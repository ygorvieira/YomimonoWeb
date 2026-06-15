using Yomimono.Domain.Common;

namespace Yomimono.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; private set; } = null!;
    public string Isbn { get; private set; } = null!;
    public int PublicationYear { get; private set; }
    public string Publisher { get; private set; } = null!;
    public string? Description { get; private set; }
    public int PageCount { get; private set; }
    public string? CoverUrl { get; private set; }
    public Guid GenreId { get; private set; }
    public Genre Genre { get; set; } = null!;
    public string? ReadingStatus { get; private set; }
    public bool IsLiked { get; private set; }
    public ICollection<BookAuthor> BookAuthors { get; private set; } = [];

    private Book() { }

    public static (Book? book, string? error) Create(
        string title, IEnumerable<Guid> authorIds, string isbn,
        int publicationYear, string publisher, Guid genreId,
        int pageCount, string? description, string? coverUrl,
        string? readingStatus = null, bool isLiked = false)
    {
        if (string.IsNullOrWhiteSpace(title))
            return (null, "O campo título é obrigatório.");
        if (!authorIds.Any())
            return (null, "É necessário selecionar pelo menos um autor.");
        if (string.IsNullOrWhiteSpace(isbn))
            return (null, "O campo ISBN é obrigatório.");
        if (string.IsNullOrWhiteSpace(publisher))
            return (null, "O campo editora é obrigatório.");
        if (genreId == Guid.Empty)
            return (null, "O campo gênero é obrigatório.");
        if (title.Length > 200)
            return (null, "O título deve ter no máximo 200 caracteres.");
        if (isbn.Length > 20)
            return (null, "O ISBN deve ter no máximo 20 caracteres.");
        if (publisher.Length > 150)
            return (null, "A editora deve ter no máximo 150 caracteres.");
        if (readingStatus is not null && readingStatus.Length > 20)
            return (null, "O status de leitura deve ter no máximo 20 caracteres.");
        if (publicationYear < 1000 || publicationYear > DateTime.UtcNow.Year)
            return (null, $"O ano de publicação deve estar entre 1000 e {DateTime.UtcNow.Year}.");
        if (pageCount <= 0)
            return (null, "O número de páginas deve ser maior que zero.");

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            Isbn = isbn,
            PublicationYear = publicationYear,
            Publisher = publisher,
            GenreId = genreId,
            Description = description,
            PageCount = pageCount,
            CoverUrl = coverUrl,
            ReadingStatus = readingStatus,
            IsLiked = isLiked,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var authorId in authorIds)
            book.BookAuthors.Add(new BookAuthor(book.Id, authorId));

        return (book, null);
    }

    public string? UpdateDetails(
        string? title, IEnumerable<Guid>? authorIds, string? isbn,
        int? publicationYear, string? publisher, Guid? genreId,
        int? pageCount, string? description, string? coverUrl,
        string? readingStatus, bool? isLiked)
    {
        if (title is not null)
        {
            if (title.Length > 200)
                return "O título deve ter no máximo 200 caracteres.";
            Title = title;
        }

        if (isbn is not null)
        {
            if (isbn.Length > 20)
                return "O ISBN deve ter no máximo 20 caracteres.";
            Isbn = isbn;
        }

        if (publisher is not null)
        {
            if (publisher.Length > 150)
                return "A editora deve ter no máximo 150 caracteres.";
            Publisher = publisher;
        }

        if (genreId.HasValue)
            GenreId = genreId.Value;

        if (publicationYear.HasValue)
        {
            var year = publicationYear.Value;
            if (year < 1000 || year > DateTime.UtcNow.Year)
                return $"O ano de publicação deve estar entre 1000 e {DateTime.UtcNow.Year}.";
            PublicationYear = year;
        }

        if (pageCount.HasValue)
        {
            if (pageCount.Value <= 0)
                return "O número de páginas deve ser maior que zero.";
            PageCount = pageCount.Value;
        }

        if (description is not null)
            Description = description;

        if (coverUrl is not null)
            CoverUrl = coverUrl;

        if (readingStatus is not null)
        {
            if (readingStatus.Length > 20)
                return "O status de leitura deve ter no máximo 20 caracteres.";
            ReadingStatus = readingStatus;
        }

        if (isLiked.HasValue)
            IsLiked = isLiked.Value;

        if (authorIds is not null)
        {
            BookAuthors.Clear();
            foreach (var authorId in authorIds)
                BookAuthors.Add(new BookAuthor(Id, authorId));
        }

        UpdatedAt = DateTime.UtcNow;
        return null;
    }

    public void UpdateStatus(string? readingStatus, bool? isLiked)
    {
        if (readingStatus is not null)
            ReadingStatus = readingStatus;
        if (isLiked.HasValue)
            IsLiked = isLiked.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
    }
}
