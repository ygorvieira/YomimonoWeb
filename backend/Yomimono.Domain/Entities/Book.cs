using Yomimono.Domain.Common;

namespace Yomimono.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public string Isbn { get; private set; } = null!;
    public int PublicationYear { get; private set; }
    public string Publisher { get; private set; } = null!;
    public string Genre { get; private set; } = null!;
    public string? Description { get; private set; }
    public int PageCount { get; private set; }
    public string? CoverUrl { get; private set; }

    private Book() { }

    public static (Book? book, string? error) Create(
        string title, string author, string isbn,
        int publicationYear, string publisher, string genre,
        int pageCount, string? description, string? coverUrl)
    {
        if (string.IsNullOrWhiteSpace(title))
            return (null, "O campo título é obrigatório.");
        if (string.IsNullOrWhiteSpace(author))
            return (null, "O campo autor é obrigatório.");
        if (string.IsNullOrWhiteSpace(isbn))
            return (null, "O campo ISBN é obrigatório.");
        if (string.IsNullOrWhiteSpace(publisher))
            return (null, "O campo editora é obrigatório.");
        if (string.IsNullOrWhiteSpace(genre))
            return (null, "O campo gênero é obrigatório.");
        if (title.Length > 200)
            return (null, "O título deve ter no máximo 200 caracteres.");
        if (author.Length > 150)
            return (null, "O autor deve ter no máximo 150 caracteres.");
        if (isbn.Length > 20)
            return (null, "O ISBN deve ter no máximo 20 caracteres.");
        if (publisher.Length > 150)
            return (null, "A editora deve ter no máximo 150 caracteres.");
        if (genre.Length > 100)
            return (null, "O gênero deve ter no máximo 100 caracteres.");
        if (publicationYear < 1000 || publicationYear > DateTime.UtcNow.Year)
            return (null, $"O ano de publicação deve estar entre 1000 e {DateTime.UtcNow.Year}.");
        if (pageCount <= 0)
            return (null, "O número de páginas deve ser maior que zero.");

        return (new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            Author = author,
            Isbn = isbn,
            PublicationYear = publicationYear,
            Publisher = publisher,
            Genre = genre,
            Description = description,
            PageCount = pageCount,
            CoverUrl = coverUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }, null);
    }

    public string? UpdateDetails(
        string? title, string? author, string? isbn,
        int? publicationYear, string? publisher, string? genre,
        int? pageCount, string? description, string? coverUrl)
    {
        if (title is not null)
        {
            if (title.Length > 200)
                return "O título deve ter no máximo 200 caracteres.";
            Title = title;
        }

        if (author is not null)
        {
            if (author.Length > 150)
                return "O autor deve ter no máximo 150 caracteres.";
            Author = author;
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

        if (genre is not null)
        {
            if (genre.Length > 100)
                return "O gênero deve ter no máximo 100 caracteres.";
            Genre = genre;
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

        UpdatedAt = DateTime.UtcNow;
        return null;
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
    }
}
