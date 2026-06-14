using Moq;
using Shouldly;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Books.Handlers;
using Yomimono.Domain.Common;
using Yomimono.Domain.Entities;

namespace Yomimono.Api.Tests.Handlers;

public class UpdateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _repositoryMock;
    private readonly Mock<IBookUniquenessChecker> _uniquenessMock;
    private readonly UpdateBookCommandHandler _handler;

    public UpdateBookCommandHandlerTests()
    {
        _repositoryMock = new Mock<IBookRepository>();
        _uniquenessMock = new Mock<IBookUniquenessChecker>();
        _handler = new UpdateBookCommandHandler(_repositoryMock.Object, _uniquenessMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingBook_ShouldReturnValidResult()
    {
        var (book, _) = Book.Create(
            "Original Title", "Original Author", "9788535902778",
            1900, "Original Publisher", "Original Genre",
            100, null, null
        );

        _repositoryMock.Setup(r => r.GetByIdAsync(book!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _uniquenessMock.Setup(r => r.IsIsbnUniqueAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var updateDto = new UpdateBookDto(
            "Updated Title", null, null, null, null, null, null, null, null
        );

        var result = await _handler.Handle(new UpdateBookCommand(book!.Id, updateDto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Title.ShouldBe("Updated Title");
        result.Data.Author.ShouldBe("Original Author");
    }

    [Fact]
    public async Task Handle_NonExistingBook_ShouldReturnInvalidResult()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var updateDto = new UpdateBookDto("Title", null, null, null, null, null, null, null, null);
        var result = await _handler.Handle(new UpdateBookCommand(Guid.NewGuid(), updateDto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_DuplicateIsbn_ShouldReturnInvalidResult()
    {
        var (book, _) = Book.Create("Title", "Author", "9788535902778", 2000, "Pub", "Gen", 100, null, null);

        _repositoryMock.Setup(r => r.GetByIdAsync(book!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _uniquenessMock.Setup(r => r.IsIsbnUniqueAsync("9788535902779", book!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var updateDto = new UpdateBookDto(null, null, "9788535902779", null, null, null, null, null, null);
        var result = await _handler.Handle(new UpdateBookCommand(book!.Id, updateDto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
