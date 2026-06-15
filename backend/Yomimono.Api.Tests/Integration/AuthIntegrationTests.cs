using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Yomimono.Application.Auth.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Api.Tests.Integration;

[Collection("Integration")]
public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static string UniqueEmail() => $"test-{Guid.NewGuid():N}@test.com";

    [Fact]
    public async Task Register_ShouldReturnTokens()
    {
        var email = UniqueEmail();
        var dto = new RegisterDto(email, "Abcd1234", "User");
        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<Result<AuthResponse>>();
        result.ShouldNotBeNull();
        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.AccessToken.ShouldNotBeNullOrEmpty();
        result.Data.RefreshToken.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_DuplicateEmail_ShouldReturnBadRequest()
    {
        var email = UniqueEmail();
        var dto = new RegisterDto(email, "Abcd1234", "User");
        await _client.PostAsJsonAsync("/api/auth/register", dto);

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<Result<AuthResponse>>();
        result.ShouldNotBeNull();
        result.Valid.ShouldBeFalse();
    }

    [Fact]
    public async Task Login_ShouldReturnTokens()
    {
        var email = UniqueEmail();
        var password = "Abcd1234";
        await _client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto(email, password, "LoginTest"));

        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(email, password));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<AuthResponse>>();
        result.ShouldNotBeNull();
        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.AccessToken.ShouldNotBeNullOrEmpty();
        result.Data.RefreshToken.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Refresh_ValidToken_ShouldReturnNewTokens()
    {
        var email = UniqueEmail();
        var password = "Abcd1234";
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto(email, password, "RefreshTest"));
        var registerResult = await registerResponse.Content
            .ReadFromJsonAsync<Result<AuthResponse>>();
        var refreshToken = registerResult!.Data!.RefreshToken;

        var response = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequestDto(refreshToken));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<AuthResponse>>();
        result.ShouldNotBeNull();
        result.Valid.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.AccessToken.ShouldNotBeNullOrEmpty();
        result.Data.RefreshToken.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Refresh_InvalidToken_ShouldReturnUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequestDto("invalid-refresh-token"));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var result = await response.Content.ReadFromJsonAsync<Result<AuthResponse>>();
        result.ShouldNotBeNull();
        result.Valid.ShouldBeFalse();
    }

    [Fact]
    public async Task Revoke_ValidToken_ShouldSucceed()
    {
        var email = UniqueEmail();
        var password = "Abcd1234";
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto(email, password, "RevokeTest"));
        var registerResult = await registerResponse.Content
            .ReadFromJsonAsync<Result<AuthResponse>>();
        var refreshToken = registerResult!.Data!.RefreshToken;

        var response = await _client.PostAsJsonAsync("/api/auth/revoke",
            new RefreshRequestDto(refreshToken));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RevokeAndRefresh_RevokedToken_ShouldReturnUnauthorized()
    {
        var email = UniqueEmail();
        var password = "Abcd1234";
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto(email, password, "RevokeAndRefresh"));
        var registerResult = await registerResponse.Content
            .ReadFromJsonAsync<Result<AuthResponse>>();
        var refreshToken = registerResult!.Data!.RefreshToken;

        await _client.PostAsJsonAsync("/api/auth/revoke",
            new RefreshRequestDto(refreshToken));

        var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequestDto(refreshToken));

        refreshResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_RotatesToken_OldTokenShouldBeRevoked()
    {
        var email = UniqueEmail();
        var password = "Abcd1234";
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto(email, password, "RotationTest"));
        var registerResult = await registerResponse.Content
            .ReadFromJsonAsync<Result<AuthResponse>>();
        var oldRefreshToken = registerResult!.Data!.RefreshToken;

        await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequestDto(oldRefreshToken));

        var oldTokenResponse = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequestDto(oldRefreshToken));

        oldTokenResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
