namespace Yomimono.Application.Books.DTOs;

public record BookDto(
    Guid Id,
    string Title,
    string Author,
    string Isbn,
    int PublicationYear,
    string Publisher,
    string Genre,
    string? Description,
    int PageCount,
    string? CoverUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateBookDto(
    string Title,
    string Author,
    string Isbn,
    int PublicationYear,
    string Publisher,
    string Genre,
    string? Description,
    int PageCount,
    string? CoverUrl
);

public record UpdateBookDto(
    string? Title,
    string? Author,
    string? Isbn,
    int? PublicationYear,
    string? Publisher,
    string? Genre,
    string? Description,
    int? PageCount,
    string? CoverUrl
);
