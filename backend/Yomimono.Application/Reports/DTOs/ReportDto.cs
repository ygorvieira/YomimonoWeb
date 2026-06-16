namespace Yomimono.Application.Reports.DTOs;

public record GenreReportDto(
    Guid GenreId,
    string GenreName,
    int BookCount,
    int LikeCount
);

public record ReportDto(
    int TotalBooks,
    int TotalRead,
    List<GenreReportDto> BooksByGenre,
    List<GenreReportDto> GenresByLikes
);
