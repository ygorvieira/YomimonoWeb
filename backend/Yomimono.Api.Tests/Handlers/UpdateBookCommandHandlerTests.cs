using Moq;
using Shouldly;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Books.Handlers;
using Yomimono.Application.Genres.Common;
using Yomimono.Domain.Entities;

namespace Yomimono.Api.Tests.Handlers;

public class UpdateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly UpdateBookCommandHandler _handler;
    private readonly Guid _genreId = Guid.NewGuid();
    private readonly Guid _authorId = Guid.NewGuid();

    public UpdateBookCommandHandlerTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _handler = new UpdateBookCommandHandler(
            _bookRepositoryMock.Object, _authorRepositoryMock.Object,
            _genreRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingBook_ShouldReturnValidResult()
    {
        var (book, _) = Book.Create(
            "Original Title", [_authorId],
            1900, "Original Publisher", [_genreId], 100, null, null, null, false
        );

        _bookRepositoryMock.Setup(r => r.GetByIdAsync(book!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var updateDto = new UpdateBookDto("Updated Title", null, null, null, null, null, null, null, null, null, null);

        var result = await _handler.Handle(new UpdateBookCommand(book.Id, updateDto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Title.ShouldBe("Updated Title");
    }

    [Fact]
    public async Task Handle_NonExistingBook_ShouldReturnInvalidResult()
    {
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var updateDto = new UpdateBookDto("Title", null, null, null, null, null, null, null, null, null, null);
        var result = await _handler.Handle(new UpdateBookCommand(Guid.NewGuid(), updateDto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_UpdateIsDigital_ShouldUpdateField()
    {
        var (book, _) = Book.Create(
            "Title", [_authorId],
            2000, "Pub", [_genreId], 100, null, null, null, false
        );

        _bookRepositoryMock.Setup(r => r.GetByIdAsync(book!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var updateDto = new UpdateBookDto(null, null, null, null, null, null, null, null, null, null, null, IsDigital: true);
        var result = await _handler.Handle(new UpdateBookCommand(book.Id, updateDto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.IsDigital.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_UpdateTradePaperback_ShouldUpdateField()
    {
        var (book, _) = Book.Create(
            "Title", [_authorId],
            2000, "Pub", [_genreId], 100, null, null, null, false
        );

        _bookRepositoryMock.Setup(r => r.GetByIdAsync(book!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var updateDto = new UpdateBookDto(null, null, null, null, null, null, null, null, null, null, null, IsTradePaperback: true, Editions: [new("2ª Edição", 2)]);
        var result = await _handler.Handle(new UpdateBookCommand(book.Id, updateDto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.IsTradePaperback.ShouldBeTrue();
        result.Data.Editions.ShouldBe([new("2ª Edição", 2)]);
    }
}
