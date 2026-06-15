namespace Yomimono.Application.Books.DTOs;

public record BookDto(
    Guid Id,
    string Title,
    Guid[] AuthorIds,
    string[] AuthorNames,
    string? Isbn,
    int PublicationYear,
    string Publisher,
    Guid GenreId,
    string GenreName,
    string? Description,
    int? PageCount,
    string? CoverUrl,
    string? ReadingStatus,
    bool IsLiked,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateBookDto(
    string Title,
    Guid[] AuthorIds,
    string? Isbn,
    int PublicationYear,
    string Publisher,
    Guid GenreId,
    string? Description,
    int? PageCount,
    string? CoverUrl,
    string? ReadingStatus,
    bool IsLiked
);

public record UpdateBookDto(
    string? Title,
    Guid[]? AuthorIds,
    string? Isbn,
    int? PublicationYear,
    string? Publisher,
    Guid? GenreId,
    string? Description,
    int? PageCount,
    string? CoverUrl,
    string? ReadingStatus,
    bool? IsLiked
);

public record UpdateBookStatusDto(
    string? ReadingStatus,
    bool? IsLiked
);
