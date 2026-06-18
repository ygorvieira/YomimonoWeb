using Moq;
using Shouldly;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.Handlers;
using Yomimono.Application.Books.Queries;
using Yomimono.Application.Common;
using Yomimono.Domain.Entities;

namespace Yomimono.Api.Tests.Handlers;

public class GetAllBooksQueryHandlerTests
{
    private readonly Mock<IBookRepository> _repositoryMock;
    private readonly GetAllBooksQueryHandler _handler;

    public GetAllBooksQueryHandlerTests()
    {
        _repositoryMock = new Mock<IBookRepository>();
        _handler = new GetAllBooksQueryHandler(_repositoryMock.Object);
    }

    private static (Book Book, Author Author) CreateTestBook(string title, string isbn)
    {
        var (author, _) = Author.Create("Author 1");
        var authorRef = author!;
        var genreId = Guid.NewGuid();
        var genre = Genre.Create("Ficção");

        var (book, _) = Book.Create(title, [authorRef.Id], isbn, 2000, "Pub", [genreId], 100, null, null, null, false);
        book!.Genres.Add(new BookGenre(book.Id, genreId));

        foreach (var ba in book.BookAuthors)
            ba.GetType().GetProperty("Author")!.SetValue(ba, authorRef);
        foreach (var bg in book.Genres)
            bg.GetType().GetProperty("Genre")!.SetValue(bg, genre);

        return (book, authorRef);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedListOfBooks()
    {
        var (book1, author1) = CreateTestBook("Book 1", "111");
        var (book2, _) = CreateTestBook("Book 2", "222");

        var books = new List<Book> { book1, book2 };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(null, null, null, 1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Book>(books, 2, 1, 50, 1, false, false));

        var result = await _handler.Handle(new GetAllBooksQuery(), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Items.Count.ShouldBe(2);
        result.Data.TotalCount.ShouldBe(2);
        result.Data.PageNumber.ShouldBe(1);
        result.Data.PageSize.ShouldBe(50);
        result.Data.TotalPages.ShouldBe(1);
        result.Data.HasNextPage.ShouldBeFalse();
        result.Data.HasPrevPage.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldReturnCorrectPage()
    {
        var allBooks = new List<Book>();
        for (int i = 1; i <= 3; i++)
        {
            var (book, _) = CreateTestBook($"Book {i}", $"{100 + i}");
            allBooks.Add(book);
        }

        var page1Books = allBooks.Take(2).ToList();

        _repositoryMock.Setup(r => r.GetAllPagedAsync(null, null, null, 1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Book>(page1Books, 3, 1, 2, 2, true, false));

        var result = await _handler.Handle(new GetAllBooksQuery(PageNumber: 1, PageSize: 2), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.Items.Count.ShouldBe(2);
        result.Data.TotalCount.ShouldBe(3);
        result.Data.TotalPages.ShouldBe(2);
        result.Data.HasNextPage.ShouldBeTrue();
        result.Data.HasPrevPage.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WithPagination_LastPage()
    {
        var allBooks = new List<Book>();
        for (int i = 1; i <= 3; i++)
        {
            var (book, _) = CreateTestBook($"Book {i}", $"{100 + i}");
            allBooks.Add(book);
        }

        var lastPageBooks = allBooks.Skip(2).Take(2).ToList();

        _repositoryMock.Setup(r => r.GetAllPagedAsync(null, null, null, 2, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Book>(lastPageBooks, 3, 2, 2, 2, false, true));

        var result = await _handler.Handle(new GetAllBooksQuery(PageNumber: 2, PageSize: 2), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.Items.Count.ShouldBe(1);
        result.Data.TotalCount.ShouldBe(3);
        result.Data.HasNextPage.ShouldBeFalse();
        result.Data.HasPrevPage.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithPagination_AndFilters()
    {
        var (book1, _) = CreateTestBook("Book 1", "111");
        var (book2, _) = CreateTestBook("Book 2", "222");

        var filteredBooks = new List<Book> { book1 };
        var genreId = book1.Genres.First().GenreId;

        _repositoryMock.Setup(r => r.GetAllPagedAsync(genreId, null, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Book>(filteredBooks, 1, 1, 10, 1, false, false));

        var result = await _handler.Handle(new GetAllBooksQuery(GenreId: genreId, PageNumber: 1, PageSize: 10), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.Items.Count.ShouldBe(1);
        result.Data.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_WithPagination_EmptyPage()
    {
        _repositoryMock.Setup(r => r.GetAllPagedAsync(null, null, null, 99, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Book>([], 3, 99, 10, 1, false, false));

        var result = await _handler.Handle(new GetAllBooksQuery(PageNumber: 99, PageSize: 10), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.Items.Count.ShouldBe(0);
    }
}
