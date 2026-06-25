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

        var readBooks = bookList.Where(b => b.ReadingStatus is "Lido" or "Relido").ToList();

        var totalRead = readBooks.Count;

        var totalPagesRead = readBooks.Sum(b => b.PageCount ?? 0);

        var totalPagesRemaining = bookList
            .Where(b => b.ReadingStatus is not "Lido" and not "Relido" && !b.IsDigital)
            .Sum(b => b.PageCount ?? 0);

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

        var authorGroups = bookList
            .SelectMany(b => b.BookAuthors.Where(ba => ba.Role == "Author").Select(ba => new { ba.AuthorId, ba.Author?.Name, Book = b }))
            .GroupBy(x => new { x.AuthorId, x.Name })
            .Select(g => new AuthorReportDto(
                g.Key.AuthorId,
                g.Key.Name ?? "",
                g.Select(x => x.Book.Id).Distinct().Count(),
                g.Where(x => x.Book.ReadingStatus is "Lido" or "Relido").Sum(x => x.Book.PageCount ?? 0),
                g.Count(x => x.Book.IsLiked)
            ))
            .ToList();

        var booksByAuthor = authorGroups.OrderByDescending(g => g.BookCount).ToList();
        var topAuthorsByLikes = authorGroups.OrderByDescending(g => g.LikeCount).Take(10).ToList();

        var report = new ReportDto(totalBooks, totalRead, totalPagesRead, totalPagesRemaining, booksByGenre, genresByLikes, booksByAuthor, topAuthorsByLikes);
        return Result<ReportDto>.Success(report);
    }
}
