using Yomimono.Domain.Entities;
using Moq;
using Shouldly;
using Yomimono.Application.Genres.Commands;
using Yomimono.Application.Genres.Common;
using Yomimono.Application.Genres.Handlers;

namespace Yomimono.Api.Tests.Handlers;

public class DeleteGenreCommandHandlerTests
{
    private readonly Mock<IGenreRepository> _repositoryMock;
    private readonly DeleteGenreCommandHandler _handler;

    public DeleteGenreCommandHandlerTests()
    {
        _repositoryMock = new Mock<IGenreRepository>();
        _handler = new DeleteGenreCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingGenre_ShouldReturnValidResult()
    {
        var genre = Genre.Create("Romance");
        _repositoryMock.Setup(r => r.GetByIdAsync(genre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(genre);

        var result = await _handler.Handle(new DeleteGenreCommand(genre.Id), CancellationToken.None);

        result.Valid.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_NotFound_ShouldReturnInvalidResult()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        var result = await _handler.Handle(new DeleteGenreCommand(Guid.NewGuid()), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
