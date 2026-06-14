using Moq;
using Shouldly;
using Yomimono.Application.Auth.Commands;
using Yomimono.Application.Auth.DTOs;
using Yomimono.Application.Auth.Handlers;
using Yomimono.Application.Common;

namespace Yomimono.Api.Tests.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _identityMock = new Mock<IIdentityService>();
        _handler = new LoginCommandHandler(_identityMock.Object);
    }

    [Fact]
    public async Task Handle_ValidLogin_ShouldReturnValidResult()
    {
        var dto = new LoginDto("user@email.com", "123456");
        var response = new AuthResponse("token123", "user@email.com", "user");

        _identityMock.Setup(x => x.LoginAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthResponse>.Success(response));

        var result = await _handler.Handle(new LoginCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Token.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ShouldReturnInvalidResult()
    {
        var dto = new LoginDto("user@email.com", "wrongpassword");

        _identityMock.Setup(x => x.LoginAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthResponse>.Failure("Email ou senha inválidos."));

        var result = await _handler.Handle(new LoginCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
