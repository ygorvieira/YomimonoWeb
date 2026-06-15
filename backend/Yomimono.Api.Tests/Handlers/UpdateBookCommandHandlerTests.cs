using Moq;
using Shouldly;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Books.Handlers;
using Yomimono.Application.Genres.Common;
using Yomimono.Domain.Common;
using Yomimono.Domain.Entities;

namespace Yomimono.Api.Tests.Handlers;

public class UpdateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IBookUniquenessChecker> _uniquenessMock;
    private readonly UpdateBookCommandHandler _handler;
    private readonly Guid _genreId = Guid.NewGuid();
    private readonly Guid _authorId = Guid.NewGuid();

    public UpdateBookCommandHandlerTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _uniquenessMock = new Mock<IBookUniquenessChecker>();
        _handler = new UpdateBookCommandHandler(
            _bookRepositoryMock.Object, _authorRepositoryMock.Object,
            _genreRepositoryMock.Object, _uniquenessMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingBook_ShouldReturnValidResult()
    {
        var genre = Genre.Create("Romance");
        var (book, _) = Book.Create(
            "Original Title", [_authorId], "9788535902778",
            1900, "Original Publisher", _genreId, 100, null, null, null, false
        );
        book!.Genre = genre;

        _bookRepositoryMock.Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
        _uniquenessMock.Setup(r => r.IsIsbnUniqueAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

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
    public async Task Handle_DuplicateIsbn_ShouldReturnInvalidResult()
    {
        var (book, _) = Book.Create("Title", [_authorId], "9788535902778", 2000, "Pub", _genreId, 100, null, null, null, false);

        _bookRepositoryMock.Setup(r => r.GetByIdAsync(book!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
        _uniquenessMock.Setup(r => r.IsIsbnUniqueAsync("9788535902779", book!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var updateDto = new UpdateBookDto(null, null, "9788535902779", null, null, null, null, null, null, null, null);
        var result = await _handler.Handle(new UpdateBookCommand(book!.Id, updateDto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
