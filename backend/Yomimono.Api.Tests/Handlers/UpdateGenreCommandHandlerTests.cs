using Yomimono.Domain.Entities;
using Moq;
using Shouldly;
using Yomimono.Application.Genres.Commands;
using Yomimono.Application.Genres.Common;
using Yomimono.Application.Genres.DTOs;
using Yomimono.Application.Genres.Handlers;

namespace Yomimono.Api.Tests.Handlers;

public class UpdateGenreCommandHandlerTests
{
    private readonly Mock<IGenreRepository> _repositoryMock;
    private readonly UpdateGenreCommandHandler _handler;

    public UpdateGenreCommandHandlerTests()
    {
        _repositoryMock = new Mock<IGenreRepository>();
        _handler = new UpdateGenreCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ShouldReturnValidResult()
    {
        var genre = Genre.Create("Romance");
        _repositoryMock.Setup(r => r.GetByIdAsync(genre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(genre);
        _repositoryMock.Setup(r => r.GetByNameAsync("Ficção", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        var dto = new UpdateGenreDto("Ficção");
        var result = await _handler.Handle(new UpdateGenreCommand(genre.Id, dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data!.Name.ShouldBe("Ficção");
    }

    [Fact]
    public async Task Handle_NotFound_ShouldReturnInvalidResult()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        var result = await _handler.Handle(
            new UpdateGenreCommand(Guid.NewGuid(), new UpdateGenreDto("Name")), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
