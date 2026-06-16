using MediatR;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Common;
using Yomimono.Application.Reports.DTOs;

namespace Yomimono.Application.Reports.Queries;

public class GetReportsQueryHandler(IBookRepository repository)
    : IRequestHandler<GetReportsQuery, Result<ReportDto>>
{
    public async Task<Result<ReportDto>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
    {
        var books = await repository.GetAllForReportsAsync(cancellationToken);
        var bookList = books.ToList();

        var totalBooks = bookList.Count;

        var totalRead = bookList.Count(b => b.ReadingStatus == "Lido" || b.ReadingStatus == "Relido");

        var genreGroups = bookList
            .SelectMany(b => b.Genres.Select(bg => new { bg.GenreId, bg.Genre?.Name, Book = b }))
            .GroupBy(x => new { x.GenreId, x.Name })
            .Select(g => new GenreReportDto(
                g.Key.GenreId,
                g.Key.Name ?? "",
                g.Count(),
                g.Sum(x => x.Book.IsLiked ? 1 : 0)
            ))
            .ToList();

        var booksByGenre = genreGroups.OrderByDescending(g => g.BookCount).ToList();
        var genresByLikes = genreGroups.OrderByDescending(g => g.LikeCount).ToList();

        var report = new ReportDto(totalBooks, totalRead, booksByGenre, genresByLikes);
        return Result<ReportDto>.Success(report);
    }
}
