using Yomimono.Domain.Common;

namespace Yomimono.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; private set; } = null!;
    public int PublicationYear { get; private set; }
    public string Publisher { get; private set; } = null!;
    public string? Description { get; private set; }
    public int? PageCount { get; private set; }
    public string? CoverUrl { get; private set; }
    public string? ReadingStatus { get; private set; }
    public bool IsLiked { get; private set; }
    public int ReReadCount { get; private set; }
    public bool IsTradePaperback { get; private set; }
    public bool IsDigital { get; private set; }
    public ICollection<BookAuthor> BookAuthors { get; private set; } = [];
    public ICollection<BookGenre> Genres { get; private set; } = [];
    public ICollection<BookEdition> BookEditions { get; private set; } = [];

    private Book() { }

    public static (Book? book, string? error) Create(
        string title, IEnumerable<Guid> authorIds,
        int publicationYear, string publisher, IEnumerable<Guid> genreIds,
        int? pageCount, string? description, string? coverUrl,
        string? readingStatus = null, bool isLiked = false,
        IEnumerable<Guid>? organizerIds = null,
        bool isTradePaperback = false, IEnumerable<(string Name, int Number)>? editions = null,
        bool isDigital = false)
    {
        if (string.IsNullOrWhiteSpace(title))
            return (null, "O campo título é obrigatório.");
        if (!authorIds.Any() && (organizerIds is null || !organizerIds.Any()))
            return (null, "É necessário selecionar pelo menos um autor ou organizador.");
        if (string.IsNullOrWhiteSpace(publisher))
            return (null, "O campo editora é obrigatório.");
        if (!genreIds.Any())
            return (null, "É necessário selecionar pelo menos um gênero.");
        if (title.Length > 200)
            return (null, "O título deve ter no máximo 200 caracteres.");
        if (publisher.Length > 150)
            return (null, "A editora deve ter no máximo 150 caracteres.");
        if (readingStatus is not null && readingStatus.Length > 20)
            return (null, "O status de leitura deve ter no máximo 20 caracteres.");
        if (publicationYear < 1000 || publicationYear > DateTime.UtcNow.Year)
            return (null, $"O ano de publicação deve estar entre 1000 e {DateTime.UtcNow.Year}.");
        if (pageCount is <= 0)
            return (null, "O número de páginas deve ser maior que zero.");
        if (editions is not null)
        {
            foreach (var (Name, Number) in editions)
            {
                if (Name.Length > 200)
                    return (null, "O nome da edição deve ter no máximo 200 caracteres.");
                if (Number <= 0)
                    return (null, "O número da edição deve ser maior que zero.");
            }
        }

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            PublicationYear = publicationYear,
            Publisher = publisher,
            Description = description,
            PageCount = pageCount,
            CoverUrl = coverUrl,
            ReadingStatus = readingStatus,
            IsLiked = isLiked,
            IsTradePaperback = isTradePaperback,
            IsDigital = isDigital,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var authorId in authorIds)
            book.BookAuthors.Add(new BookAuthor(book.Id, authorId, "Author"));

        if (organizerIds is not null)
        {
            foreach (var organizerId in organizerIds)
                book.BookAuthors.Add(new BookAuthor(book.Id, organizerId, "Organizer"));
        }

        foreach (var genreId in genreIds)
            book.Genres.Add(new BookGenre(book.Id, genreId));

        if (editions is not null)
        {
            foreach (var (Name, Number) in editions)
                book.BookEditions.Add(new BookEdition(book.Id, Name, Number));
        }

        return (book, null);
    }

    public string? UpdateDetails(
        string? title, IEnumerable<Guid>? authorIds,
        int? publicationYear, string? publisher, IEnumerable<Guid>? genreIds,
        int? pageCount, string? description, string? coverUrl,
        string? readingStatus, bool? isLiked,
        IEnumerable<Guid>? organizerIds = null,
        bool? isTradePaperback = null, IEnumerable<(string Name, int Number)>? editions = null,
        bool? isDigital = null)
    {
        if (title is not null)
        {
            if (title.Length > 200)
                return "O título deve ter no máximo 200 caracteres.";
            Title = title;
        }

        if (publisher is not null)
        {
            if (publisher.Length > 150)
                return "A editora deve ter no máximo 150 caracteres.";
            Publisher = publisher;
        }

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

        if (isTradePaperback.HasValue)
            IsTradePaperback = isTradePaperback.Value;

        if (isDigital.HasValue)
            IsDigital = isDigital.Value;

        if (authorIds is not null || organizerIds is not null)
        {
            BookAuthors.Clear();

            if (authorIds is not null)
            {
                foreach (var authorId in authorIds)
                    BookAuthors.Add(new BookAuthor(Id, authorId, "Author"));
            }

            if (organizerIds is not null)
            {
                foreach (var organizerId in organizerIds)
                    BookAuthors.Add(new BookAuthor(Id, organizerId, "Organizer"));
            }
        }

        if (genreIds is not null)
        {
            Genres.Clear();
            foreach (var genreId in genreIds)
                Genres.Add(new BookGenre(Id, genreId));
        }

        if (editions is not null)
        {
            BookEditions.Clear();
            foreach (var edition in editions)
                BookEditions.Add(new BookEdition(Id, edition.Name, edition.Number));
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
