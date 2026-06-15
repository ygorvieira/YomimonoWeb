using Yomimono.Domain.Entities;
using Moq;
using Shouldly;
using Yomimono.Application.Authors.Commands;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Authors.Handlers;

namespace Yomimono.Api.Tests.Handlers;

public class DeleteAuthorCommandHandlerTests
{
    private readonly Mock<IAuthorRepository> _repositoryMock;
    private readonly DeleteAuthorCommandHandler _handler;

    public DeleteAuthorCommandHandlerTests()
    {
        _repositoryMock = new Mock<IAuthorRepository>();
        _handler = new DeleteAuthorCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingAuthor_ShouldReturnValidResult()
    {
        var (author, _) = Author.Create("Machado de Assis");
        _repositoryMock.Setup(r => r.GetByIdAsync(author!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _handler.Handle(new DeleteAuthorCommand(author!.Id), CancellationToken.None);

        result.Valid.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_NotFound_ShouldReturnInvalidResult()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var result = await _handler.Handle(new DeleteAuthorCommand(Guid.NewGuid()), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
