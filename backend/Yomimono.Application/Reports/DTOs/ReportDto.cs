namespace Yomimono.Application.Reports.DTOs;

public record GenreReportDto(
    Guid GenreId,
    string GenreName,
    int BookCount,
    int LikeCount
);

public record AuthorReportDto(
    Guid AuthorId,
    string AuthorName,
    int BookCount,
    int TotalPagesRead,
    int LikeCount
);

public record ReportDto(
    int TotalBooks,
    int TotalRead,
    int TotalPagesRead,
    List<GenreReportDto> BooksByGenre,
    List<GenreReportDto> GenresByLikes,
    List<AuthorReportDto> BooksByAuthor,
    List<AuthorReportDto> TopAuthorsByLikes
);
