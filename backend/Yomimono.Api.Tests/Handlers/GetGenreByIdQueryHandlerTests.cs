using Yomimono.Domain.Entities;
using Moq;
using Shouldly;
using Yomimono.Application.Genres.Common;
using Yomimono.Application.Genres.Handlers;
using Yomimono.Application.Genres.Queries;

namespace Yomimono.Api.Tests.Handlers;

public class GetGenreByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingGenre_ShouldReturnGenre()
    {
        var repositoryMock = new Mock<IGenreRepository>();
        var genre = Genre.Create("Romance");
        repositoryMock.Setup(r => r.GetByIdAsync(genre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(genre);

        var handler = new GetGenreByIdQueryHandler(repositoryMock.Object);
        var result = await handler.Handle(new GetGenreByIdQuery(genre.Id), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe("Romance");
    }

    [Fact]
    public async Task Handle_NonExistingGenre_ShouldReturnNotFound()
    {
        var repositoryMock = new Mock<IGenreRepository>();
        repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        var handler = new GetGenreByIdQueryHandler(repositoryMock.Object);
        var result = await handler.Handle(new GetGenreByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
