using Yomimono.Domain.Entities;
using Moq;
using Shouldly;
using Yomimono.Application.Auth.Commands;
using Yomimono.Application.Auth.DTOs;
using Yomimono.Application.Auth.Handlers;
using Yomimono.Application.Common;

namespace Yomimono.Api.Tests.Handlers;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _identityMock = new Mock<IIdentityService>();
        _handler = new RegisterCommandHandler(_identityMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRegistration_ShouldReturnValidResult()
    {
        var dto = new RegisterDto("user@email.com", "123456", "user");
        var response = new AuthResponse("access-token", "refresh-token", "user@email.com", "user");

        _identityMock.Setup(x => x.RegisterAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthResponse>.Created(response));

        var result = await _handler.Handle(new RegisterCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Email.ShouldBe("user@email.com");
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ShouldReturnInvalidResult()
    {
        var dto = new RegisterDto("existing@email.com", "123456", "user");

        _identityMock.Setup(x => x.RegisterAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthResponse>.Failure("Já existe um usuário com este email."));

        var result = await _handler.Handle(new RegisterCommand(dto), CancellationToken.None);

        result.Valid.ShouldBeFalse();
    }
}
