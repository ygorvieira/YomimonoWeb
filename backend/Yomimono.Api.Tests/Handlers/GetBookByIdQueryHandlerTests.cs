using Moq;
using Shouldly;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.Handlers;
using Yomimono.Application.Books.Queries;
using Yomimono.Domain.Entities;

namespace Yomimono.Api.Tests.Handlers;

public class GetBookByIdQueryHandlerTests
{
    private readonly Mock<IBookRepository> _repositoryMock;
    private readonly GetBookByIdQueryHandler _handler;

    public GetBookByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IBookRepository>();
        _handler = new GetBookByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingBook_ShouldReturnValidResult()
    {
        var (book, _) = Book.Create(
            "Dom Casmurro", "Machado de Assis", "9788535902778",
            1899, "Garnier", "Romance", 256, null, null
        );

        _repositoryMock.Setup(r => r.GetByIdAsync(book!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler.Handle(new GetBookByIdQuery(book!.Id), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Id.ShouldBe(book.Id);
    }

    [Fact]
    public async Task Handle_NonExistingBook_ShouldReturnInvalidResult()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var result = await _handler.Handle(new GetBookByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
