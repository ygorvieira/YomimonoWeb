using Moq;
using Shouldly;
using Yomimono.Application.Auth.Commands;
using Yomimono.Application.Auth.DTOs;
using Yomimono.Application.Common;
using Yomimono.Application.Auth.Handlers;

namespace Yomimono.Api.Tests.Handlers;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _identityMock = new Mock<IIdentityService>();
        _handler = new RefreshTokenCommandHandler(_identityMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRefreshToken_ShouldReturnValidResult()
    {
        var dto = new RefreshRequestDto("valid-refresh-token");
        var response = new AuthResponse("new-access-token", "new-refresh-token", "user@email.com", "user");

        _identityMock.Setup(x => x.RefreshTokenAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthResponse>.Success(response));

        var result = await _handler.Handle(new RefreshTokenCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.AccessToken.ShouldNotBeNullOrEmpty();
        result.Data.RefreshToken.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_ExpiredRefreshToken_ShouldReturnInvalidResult()
    {
        var dto = new RefreshRequestDto("expired-refresh-token");

        _identityMock.Setup(x => x.RefreshTokenAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthResponse>.Failure("Refresh token inválido ou expirado."));

        var result = await _handler.Handle(new RefreshTokenCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
