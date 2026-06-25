using Yomimono.Domain.Entities;
using Moq;
using Shouldly;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Authors.Handlers;
using Yomimono.Application.Authors.Queries;

namespace Yomimono.Api.Tests.Handlers;

public class GetAllAuthorsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAllAuthors()
    {
        var repositoryMock = new Mock<IAuthorRepository>();
        var (author1, _) = Author.Create("Machado de Assis");
        var (author2, _) = Author.Create("Clarice Lispector");
        repositoryMock.Setup(r => r.GetAllAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { author1!, author2! });

        var handler = new GetAllAuthorsQueryHandler(repositoryMock.Object);
        var result = await handler.Handle(new GetAllAuthorsQuery(), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(2);
    }
}
