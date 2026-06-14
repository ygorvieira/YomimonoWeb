using Moq;
using Shouldly;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Books.Handlers;
using Yomimono.Domain.Common;
using Yomimono.Domain.Entities;

namespace Yomimono.Api.Tests.Handlers;

public class CreateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _repositoryMock;
    private readonly Mock<IBookUniquenessChecker> _uniquenessMock;
    private readonly CreateBookCommandHandler _handler;

    public CreateBookCommandHandlerTests()
    {
        _repositoryMock = new Mock<IBookRepository>();
        _uniquenessMock = new Mock<IBookUniquenessChecker>();
        _handler = new CreateBookCommandHandler(_repositoryMock.Object, _uniquenessMock.Object);
    }

    [Fact]
    public async Task Handle_ValidBook_ShouldReturnValidResult()
    {
        var dto = new CreateBookDto(
            "Dom Casmurro", "Machado de Assis", "9788535902778",
            1899, "Editora Garnier", "Romance",
            "Um clássico da literatura brasileira.", 256, null
        );

        _uniquenessMock.Setup(r => r.IsIsbnUniqueAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Title.ShouldBe("Dom Casmurro");
    }

    [Fact]
    public async Task Handle_DuplicateIsbn_ShouldReturnInvalidResult()
    {
        var dto = new CreateBookDto(
            "Dom Casmurro", "Machado de Assis", "9788535902778",
            1899, "Editora Garnier", "Romance", null, 256, null
        );

        _uniquenessMock.Setup(r => r.IsIsbnUniqueAsync("9788535902778", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_InvalidData_ShouldReturnInvalidResult()
    {
        var dto = new CreateBookDto(
            "", "Autor", "123", 2024, "Pub", "Gen", null, 100, null
        );

        _uniquenessMock.Setup(r => r.IsIsbnUniqueAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
