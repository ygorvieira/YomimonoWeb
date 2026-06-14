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
        var (book1, _) = Book.Create("Book 1", "Author 1", "111", 2000, "Pub", "Ficção", 100, null, null);
        var (book2, _) = Book.Create("Book 2", "Author 2", "222", 2001, "Pub", "Romance", 200, null, null);

        var books = new List<Book> { book1!, book2! };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);

        var result = await _handler.Handle(new GetAllBooksQuery(), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(2);
    }
}
