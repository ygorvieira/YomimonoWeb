using Moq;
using Shouldly;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Reports.Queries;
using Yomimono.Domain.Entities;

namespace Yomimono.Api.Tests.Handlers;

public class GetReportsQueryHandlerTests
{
    private readonly Mock<IBookRepository> _repositoryMock;
    private readonly GetReportsQueryHandler _handler;

    public GetReportsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IBookRepository>();
        _handler = new GetReportsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTotalPagesRead_SumOfLidoAndRelido()
    {
        var (book1, author1) = CreateBook("Book 1", 200, "Lido");
        var (book2, _) = CreateBook("Book 2", 300, "Relido", existingAuthor: author1);
        var (book3, _) = CreateBook("Book 3", 100, "Lendo", existingAuthor: author1);

        _repositoryMock.Setup(r => r.GetAllForReportsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book> { book1, book2, book3 });

        var result = await _handler.Handle(new GetReportsQuery(), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.TotalPagesRead.ShouldBe(500);
    }

    [Fact]
    public async Task Handle_ShouldIgnoreNullPageCountInTotalPagesRead()
    {
        var (book1, author1) = CreateBook("Book 1", null, "Lido");
        var (book2, _) = CreateBook("Book 2", 150, "Lido", existingAuthor: author1);

        _repositoryMock.Setup(r => r.GetAllForReportsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book> { book1, book2 });

        var result = await _handler.Handle(new GetReportsQuery(), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.TotalPagesRead.ShouldBe(150);
    }

    [Fact]
    public async Task Handle_ShouldReturnBooksByAuthor_OrderedByBookCountDesc()
    {
        var (book1, author1) = CreateBook("Book 1", 100, "Lido", isLiked: true);
        var (book2, _) = CreateBook("Book 2", 200, "Lido", existingAuthor: author1);
        var author2 = CreateAuthor("Author B");
        var (book3, _) = CreateBook("Book 3", 300, "Relido", isLiked: true, existingAuthor: author2);

        _repositoryMock.Setup(r => r.GetAllForReportsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book> { book1, book2, book3 });

        var result = await _handler.Handle(new GetReportsQuery(), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.BooksByAuthor.Count.ShouldBe(2);
        result.Data.BooksByAuthor[0].AuthorName.ShouldBe("Author A");
        result.Data.BooksByAuthor[0].BookCount.ShouldBe(2);
        result.Data.BooksByAuthor[0].TotalPagesRead.ShouldBe(300);
        result.Data.BooksByAuthor[0].LikeCount.ShouldBe(1);
        result.Data.BooksByAuthor[1].AuthorName.ShouldBe("Author B");
        result.Data.BooksByAuthor[1].BookCount.ShouldBe(1);
        result.Data.BooksByAuthor[1].TotalPagesRead.ShouldBe(300);
        result.Data.BooksByAuthor[1].LikeCount.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnTopAuthorsByLikes_OrderedByLikeCountDesc()
    {
        var (book1, author1) = CreateBook("Book 1", 100, "Lido", isLiked: true);
        var author2 = CreateAuthor("Author B");
        var (book2, _) = CreateBook("Book 2", 200, "Lido", isLiked: true, existingAuthor: author2);
        var (book3, _) = CreateBook("Book 3", 300, "Lido", isLiked: true, existingAuthor: author2);

        _repositoryMock.Setup(r => r.GetAllForReportsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book> { book1, book2, book3 });

        var result = await _handler.Handle(new GetReportsQuery(), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.TopAuthorsByLikes.Count.ShouldBe(2);
        result.Data.TopAuthorsByLikes[0].AuthorName.ShouldBe("Author B");
        result.Data.TopAuthorsByLikes[0].LikeCount.ShouldBe(2);
        result.Data.TopAuthorsByLikes[1].AuthorName.ShouldBe("Author A");
        result.Data.TopAuthorsByLikes[1].LikeCount.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_ShouldHandleCoAuthorship()
    {
        var author1 = CreateAuthor("Author A");
        var author2 = CreateAuthor("Author B");
        var genreId = Guid.NewGuid();

        var (book, _) = Book.Create("Book 1", [author1.Id, author2.Id], 2000, "Pub", [genreId], 200, null, null, "Lido", true);
        book!.Genres.Add(new BookGenre(book.Id, genreId));
        SetGenreNavigation(book);
        SetAuthorNavigation(book, author1, author2);

        _repositoryMock.Setup(r => r.GetAllForReportsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book> { book });

        var result = await _handler.Handle(new GetReportsQuery(), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.BooksByAuthor.Count.ShouldBe(2);
        result.Data.BooksByAuthor[0].BookCount.ShouldBe(1);
        result.Data.BooksByAuthor[1].BookCount.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnZeroTotalPagesRead_WhenNoBooksRead()
    {
        var (book, _) = CreateBook("Book 1", 200, "Lendo");

        _repositoryMock.Setup(r => r.GetAllForReportsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book> { book });

        var result = await _handler.Handle(new GetReportsQuery(), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.TotalPagesRead.ShouldBe(0);
    }

    private static Author CreateAuthor(string name)
    {
        var (author, _) = Author.Create(name);
        return author!;
    }

    private static (Book, Author) CreateBook(string title, int? pageCount, string readingStatus, bool isLiked = false, Author? existingAuthor = null)
    {
        var author = existingAuthor ?? CreateAuthor("Author A");
        var genreId = Guid.NewGuid();

        var (book, _) = Book.Create(title, [author.Id], 2000, "Pub", [genreId], pageCount, null, null, readingStatus, isLiked);
        book!.Genres.Add(new BookGenre(book.Id, genreId));
        SetGenreNavigation(book);
        SetAuthorNavigation(book, author);

        return (book, author);
    }

    private static void SetAuthorNavigation(Book book, params Author[] authors)
    {
        foreach (var ba in book.BookAuthors)
        {
            var author = authors.FirstOrDefault(a => a.Id == ba.AuthorId);
            if (author is not null)
                ba.GetType().GetProperty("Author")!.SetValue(ba, author);
        }
    }

    private static void SetGenreNavigation(Book book)
    {
        foreach (var bg in book.Genres)
        {
            var genre = Genre.Create("Genre");
            bg.GetType().GetProperty("Genre")!.SetValue(bg, genre);
        }
    }
}
