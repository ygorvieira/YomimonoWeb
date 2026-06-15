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

public class CreateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IBookUniquenessChecker> _uniquenessMock;
    private readonly CreateBookCommandHandler _handler;
    private readonly Guid _genreId = Guid.NewGuid();
    private readonly Guid _authorId = Guid.NewGuid();

    public CreateBookCommandHandlerTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _uniquenessMock = new Mock<IBookUniquenessChecker>();
        _handler = new CreateBookCommandHandler(
            _bookRepositoryMock.Object, _authorRepositoryMock.Object,
            _genreRepositoryMock.Object, _uniquenessMock.Object);
    }

    [Fact]
    public async Task Handle_ValidBook_ShouldReturnValidResult()
    {
        var dto = new CreateBookDto(
            "Dom Casmurro", [_authorId], "9788535902778",
            1899, "Editora Garnier", _genreId, null, 256, null, null, false
        );

        var (author, _) = Author.Create("Machado de Assis");

        _uniquenessMock.Setup(r => r.IsIsbnUniqueAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _genreRepositoryMock.Setup(r => r.GetByIdAsync(_genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Genre.Create("Romance"));
        _authorRepositoryMock.Setup(r => r.GetByIdAsync(_authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Title.ShouldBe("Dom Casmurro");
    }

    [Fact]
    public async Task Handle_DuplicateIsbn_ShouldReturnInvalidResult()
    {
        var dto = new CreateBookDto(
            "Dom Casmurro", [_authorId], "9788535902778",
            1899, "Editora Garnier", _genreId, null, 256, null, null, false
        );

        _uniquenessMock.Setup(r => r.IsIsbnUniqueAsync("9788535902778", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_InvalidGenre_ShouldReturnInvalidResult()
    {
        var dto = new CreateBookDto(
            "Dom Casmurro", [_authorId], "9788535902778",
            1899, "Editora Garnier", _genreId, null, 256, null, null, false
        );

        _uniquenessMock.Setup(r => r.IsIsbnUniqueAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _genreRepositoryMock.Setup(r => r.GetByIdAsync(_genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        var result = await _handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_NullIsbn_ShouldReturnValidResult()
    {
        var dto = new CreateBookDto(
            "Sem ISBN", [_authorId], null,
            2024, "Editora", _genreId, null, 100, null, null, false
        );

        var (author, _) = Author.Create("Autor");

        _genreRepositoryMock.Setup(r => r.GetByIdAsync(_genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Genre.Create("Ficção"));
        _authorRepositoryMock.Setup(r => r.GetByIdAsync(_authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Isbn.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_NullPageCount_ShouldReturnValidResult()
    {
        var dto = new CreateBookDto(
            "Sem páginas", [_authorId], "9788535902778",
            2024, "Editora", _genreId, null, null, null, null, false
        );

        var (author, _) = Author.Create("Autor");

        _uniquenessMock.Setup(r => r.IsIsbnUniqueAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _genreRepositoryMock.Setup(r => r.GetByIdAsync(_genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Genre.Create("Ficção"));
        _authorRepositoryMock.Setup(r => r.GetByIdAsync(_authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.PageCount.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_NullIsbn_ShouldSkipUniquenessCheck()
    {
        var dto = new CreateBookDto(
            "Sem ISBN", [_authorId], null,
            2024, "Editora", _genreId, null, 100, null, null, false
        );

        var (author, _) = Author.Create("Autor");

        _genreRepositoryMock.Setup(r => r.GetByIdAsync(_genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Genre.Create("Ficção"));
        _authorRepositoryMock.Setup(r => r.GetByIdAsync(_authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        _uniquenessMock.Verify(r => r.IsIsbnUniqueAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()), Times.Never);
    }
}
