using Yomimono.Domain.Entities;
using Moq;
using Shouldly;
using Yomimono.Application.Authors.Commands;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Authors.DTOs;
using Yomimono.Application.Authors.Handlers;

namespace Yomimono.Api.Tests.Handlers;

public class UpdateAuthorCommandHandlerTests
{
    private readonly Mock<IAuthorRepository> _repositoryMock;
    private readonly UpdateAuthorCommandHandler _handler;

    public UpdateAuthorCommandHandlerTests()
    {
        _repositoryMock = new Mock<IAuthorRepository>();
        _handler = new UpdateAuthorCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ShouldReturnValidResult()
    {
        var (author, _) = Author.Create("Machado de Assis");
        _repositoryMock.Setup(r => r.GetByIdAsync(author!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _repositoryMock.Setup(r => r.GetByNameAsync("Machado de Assis Jr.", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var dto = new UpdateAuthorDto("Machado de Assis Jr.");
        var result = await _handler.Handle(new UpdateAuthorCommand(author!.Id, dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data!.Name.ShouldBe("Machado de Assis Jr.");
    }

    [Fact]
    public async Task Handle_NotFound_ShouldReturnInvalidResult()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var result = await _handler.Handle(
            new UpdateAuthorCommand(Guid.NewGuid(), new UpdateAuthorDto("Name")), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
