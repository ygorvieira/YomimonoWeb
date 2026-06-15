using Yomimono.Domain.Entities;
using Moq;
using Shouldly;
using Yomimono.Application.Genres.Commands;
using Yomimono.Application.Genres.Common;
using Yomimono.Application.Genres.DTOs;
using Yomimono.Application.Genres.Handlers;

namespace Yomimono.Api.Tests.Handlers;

public class CreateGenreCommandHandlerTests
{
    private readonly Mock<IGenreRepository> _repositoryMock;
    private readonly CreateGenreCommandHandler _handler;

    public CreateGenreCommandHandlerTests()
    {
        _repositoryMock = new Mock<IGenreRepository>();
        _handler = new CreateGenreCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidGenre_ShouldReturnValidResult()
    {
        var dto = new CreateGenreDto("Romance");
        _repositoryMock.Setup(r => r.GetByNameAsync("Romance", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        var result = await _handler.Handle(new CreateGenreCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe("Romance");
    }

    [Fact]
    public async Task Handle_DuplicateName_ShouldReturnInvalidResult()
    {
        var dto = new CreateGenreDto("Romance");
        var existing = Genre.Create("Romance");
        _repositoryMock.Setup(r => r.GetByNameAsync("Romance", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.Handle(new CreateGenreCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_EmptyName_ShouldReturnInvalidResult()
    {
        var dto = new CreateGenreDto("");

        var result = await _handler.Handle(new CreateGenreCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
