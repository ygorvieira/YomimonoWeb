using Yomimono.Domain.Entities;
using Moq;
using Shouldly;
using Yomimono.Application.Authors.Commands;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Authors.DTOs;
using Yomimono.Application.Authors.Handlers;

namespace Yomimono.Api.Tests.Handlers;

public class CreateAuthorCommandHandlerTests
{
    private readonly Mock<IAuthorRepository> _repositoryMock;
    private readonly CreateAuthorCommandHandler _handler;

    public CreateAuthorCommandHandlerTests()
    {
        _repositoryMock = new Mock<IAuthorRepository>();
        _handler = new CreateAuthorCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidAuthor_ShouldReturnValidResult()
    {
        var dto = new CreateAuthorDto("Machado de Assis");
        _repositoryMock.Setup(r => r.GetByNameAsync("Machado de Assis", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var result = await _handler.Handle(new CreateAuthorCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe("Machado de Assis");
    }

    [Fact]
    public async Task Handle_DuplicateName_ShouldReturnInvalidResult()
    {
        var dto = new CreateAuthorDto("Machado de Assis");
        var (existing, _) = Author.Create("Machado de Assis");
        _repositoryMock.Setup(r => r.GetByNameAsync("Machado de Assis", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.Handle(new CreateAuthorCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_EmptyName_ShouldReturnInvalidResult()
    {
        var dto = new CreateAuthorDto("");

        var result = await _handler.Handle(new CreateAuthorCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
