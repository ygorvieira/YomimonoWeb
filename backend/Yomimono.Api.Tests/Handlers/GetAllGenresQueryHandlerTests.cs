using Yomimono.Domain.Entities;
using Moq;
using Shouldly;
using Yomimono.Application.Genres.Common;
using Yomimono.Application.Genres.Handlers;
using Yomimono.Application.Genres.Queries;

namespace Yomimono.Api.Tests.Handlers;

public class GetAllGenresQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAllGenres()
    {
        var repositoryMock = new Mock<IGenreRepository>();
        var genre1 = Genre.Create("Romance");
        var genre2 = Genre.Create("Ficção");
        repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { genre1, genre2 });

        var handler = new GetAllGenresQueryHandler(repositoryMock.Object);
        var result = await handler.Handle(new GetAllGenresQuery(), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Count().ShouldBe(2);
    }
}
