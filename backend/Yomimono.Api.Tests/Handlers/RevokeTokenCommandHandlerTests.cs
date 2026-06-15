using Moq;
using Shouldly;
using Yomimono.Application.Auth.Commands;
using Yomimono.Application.Auth.DTOs;
using Yomimono.Application.Common;
using Yomimono.Application.Auth.Handlers;

namespace Yomimono.Api.Tests.Handlers;

public class RevokeTokenCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityMock;
    private readonly RevokeTokenCommandHandler _handler;

    public RevokeTokenCommandHandlerTests()
    {
        _identityMock = new Mock<IIdentityService>();
        _handler = new RevokeTokenCommandHandler(_identityMock.Object);
    }

    [Fact]
    public async Task Handle_ValidToken_ShouldReturnValidResult()
    {
        var dto = new RefreshRequestDto("valid-refresh-token");

        _identityMock.Setup(x => x.RevokeTokenAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(new RevokeTokenCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_InvalidToken_ShouldStillReturnValidResult()
    {
        var dto = new RefreshRequestDto("invalid-refresh-token");

        _identityMock.Setup(x => x.RevokeTokenAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(new RevokeTokenCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
    }
}
