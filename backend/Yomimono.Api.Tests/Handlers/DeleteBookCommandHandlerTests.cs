using Moq;
using Shouldly;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.Handlers;
using Yomimono.Domain.Entities;

namespace Yomimono.Api.Tests.Handlers;

public class DeleteBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _repositoryMock;
    private readonly DeleteBookCommandHandler _handler;

    public DeleteBookCommandHandlerTests()
    {
        _repositoryMock = new Mock<IBookRepository>();
        _handler = new DeleteBookCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingBook_ShouldReturnValidResult()
    {
        var (book, _) = Book.Create("Title", [Guid.NewGuid()], 2000, "Pub", [Guid.NewGuid()], 100, null, null, null, false);

        _repositoryMock.Setup(r => r.GetByIdAsync(book!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler.Handle(new DeleteBookCommand(book!.Id), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_NonExistingBook_ShouldReturnInvalidResult()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var result = await _handler.Handle(new DeleteBookCommand(Guid.NewGuid()), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
