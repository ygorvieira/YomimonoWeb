using Yomimono.Domain.Entities;
using Moq;
using Shouldly;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Authors.Handlers;
using Yomimono.Application.Authors.Queries;

namespace Yomimono.Api.Tests.Handlers;

public class GetAuthorByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingAuthor_ShouldReturnAuthor()
    {
        var repositoryMock = new Mock<IAuthorRepository>();
        var (author, _) = Author.Create("Machado de Assis");
        repositoryMock.Setup(r => r.GetByIdAsync(author!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var handler = new GetAuthorByIdQueryHandler(repositoryMock.Object);
        var result = await handler.Handle(new GetAuthorByIdQuery(author!.Id), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe("Machado de Assis");
    }

    [Fact]
    public async Task Handle_NonExistingAuthor_ShouldReturnNotFound()
    {
        var repositoryMock = new Mock<IAuthorRepository>();
        repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var handler = new GetAuthorByIdQueryHandler(repositoryMock.Object);
        var result = await handler.Handle(new GetAuthorByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
