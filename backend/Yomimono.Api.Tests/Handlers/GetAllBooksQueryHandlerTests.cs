using Moq;
using Shouldly;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.Handlers;
using Yomimono.Application.Books.Queries;
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

    [Fact]
    public async Task Handle_ShouldReturnListOfBooks()
    {
        var authorId = Guid.NewGuid();
        var genreId = Guid.NewGuid();
        var genre = Genre.Create("Ficção");
        var (author, _) = Author.Create("Author 1");

        var (book1, _) = Book.Create("Book 1", [authorId], "111", 2000, "Pub", [genreId], 100, null, null, null, false);
        book1!.Genres.Add(new BookGenre(book1.Id, genreId));
        foreach (var ba in book1.BookAuthors)
            ba.GetType().GetProperty("Author")!.SetValue(ba, author);
        foreach (var bg in book1.Genres)
            bg.GetType().GetProperty("Genre")!.SetValue(bg, genre);

        var (book2, _) = Book.Create("Book 2", [authorId], "222", 2001, "Pub", [genreId], 200, null, null, null, false);
        book2!.Genres.Add(new BookGenre(book2.Id, genreId));
        foreach (var ba in book2.BookAuthors)
            ba.GetType().GetProperty("Author")!.SetValue(ba, author);
        foreach (var bg in book2.Genres)
            bg.GetType().GetProperty("Genre")!.SetValue(bg, genre);

        var books = new List<Book> { book1, book2 };

        _repositoryMock.Setup(r => r.GetAllAsync(null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);

        var result = await _handler.Handle(new GetAllBooksQuery(), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(2);
    }
}
