namespace Yomimono.Application.Books.DTOs;

public record BookEditionDto(string Name, int Number);

public record BookDto(
    Guid Id,
    string Title,
    Guid[] AuthorIds,
    string[] AuthorNames,
    Guid[] OrganizerIds,
    string[] OrganizerNames,
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
    bool IsTradePaperback,
    BookEditionDto[] Editions,
    bool IsDigital,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateBookDto(
    string Title,
    Guid[] AuthorIds,
    int PublicationYear,
    string Publisher,
    Guid[] GenreIds,
    string? Description,
    int? PageCount,
    string? CoverUrl,
    string? ReadingStatus,
    bool IsLiked,
    Guid[]? OrganizerIds,
    bool IsTradePaperback = false,
    BookEditionDto[]? Editions = null,
    bool IsDigital = false
);

public record UpdateBookDto(
    string? Title,
    Guid[]? AuthorIds,
    int? PublicationYear,
    string? Publisher,
    Guid[]? GenreIds,
    string? Description,
    int? PageCount,
    string? CoverUrl,
    string? ReadingStatus,
    bool? IsLiked,
    Guid[]? OrganizerIds,
    bool? IsTradePaperback = null,
    BookEditionDto[]? Editions = null,
    bool? IsDigital = null
);

public record UpdateBookStatusDto(
    string? ReadingStatus,
    bool? IsLiked
);
