namespace Yomimono.Application.Books.DTOs;

public record BookDto(
    Guid Id,
    string Title,
    Guid[] AuthorIds,
    string[] AuthorNames,
    Guid[] OrganizerIds,
    string[] OrganizerNames,
    string? Isbn,
    int PublicationYear,
    string Publisher,
    Guid[] GenreIds,
    string[] GenreNames,
    string? Description,
    int? PageCount,
    string? CoverUrl,
    string? ReadingStatus,
    bool IsLiked,
    int ReReadCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateBookDto(
    string Title,
    Guid[] AuthorIds,
    string? Isbn,
    int PublicationYear,
    string Publisher,
    Guid[] GenreIds,
    string? Description,
    int? PageCount,
    string? CoverUrl,
    string? ReadingStatus,
    bool IsLiked,
    Guid[]? OrganizerIds
);

public record UpdateBookDto(
    string? Title,
    Guid[]? AuthorIds,
    string? Isbn,
    int? PublicationYear,
    string? Publisher,
    Guid[]? GenreIds,
    string? Description,
    int? PageCount,
    string? CoverUrl,
    string? ReadingStatus,
    bool? IsLiked,
    Guid[]? OrganizerIds
);

public record UpdateBookStatusDto(
    string? ReadingStatus,
    bool? IsLiked
);
