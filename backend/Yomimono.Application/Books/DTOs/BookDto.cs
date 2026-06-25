namespace Yomimono.Application.Books.DTOs;

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
    string? TradeEdition,
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
    string? TradeEdition = null,
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
    string? TradeEdition = null,
    bool? IsDigital = null
);

public record UpdateBookStatusDto(
    string? ReadingStatus,
    bool? IsLiked
);
