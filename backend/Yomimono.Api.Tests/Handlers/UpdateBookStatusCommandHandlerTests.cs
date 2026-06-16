using Moq;
using Shouldly;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Books.Handlers;
using Yomimono.Domain.Entities;

namespace Yomimono.Api.Tests.Handlers;

public class UpdateBookStatusCommandHandlerTests
{
    private readonly Mock<IBookRepository> _repositoryMock;
    private readonly UpdateBookStatusCommandHandler _handler;

    public UpdateBookStatusCommandHandlerTests()
    {
        _repositoryMock = new Mock<IBookRepository>();
        _handler = new UpdateBookStatusCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidStatusUpdate_ShouldReturnValidResult()
    {
        var genreId = Guid.NewGuid();
        var genre = Genre.Create("Romance");
        var (book, _) = Book.Create("Title", [Guid.NewGuid()], "111", 2000, "Pub", [genreId], 100, null, null, null, false);
        book!.Genres.Add(new BookGenre(book.Id, genreId));
        foreach (var bg in book.Genres)
            bg.GetType().GetProperty("Genre")!.SetValue(bg, genre);

        _repositoryMock.Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var dto = new UpdateBookStatusDto("Lendo", true);
        var result = await _handler.Handle(new UpdateBookStatusCommand(book.Id, dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.ReadingStatus.ShouldBe("Lendo");
        result.Data.IsLiked.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_NotFound_ShouldReturnInvalidResult()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var result = await _handler.Handle(
            new UpdateBookStatusCommand(Guid.NewGuid(), new UpdateBookStatusDto("Lendo", null)),
            CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
