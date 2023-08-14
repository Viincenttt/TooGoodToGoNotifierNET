using FluentAssertions;
using FluentAssertions.Extensions;
using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication;
using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication.Cache;

namespace TooGoodToGoNotifier.Application.Tests.TooGoodToGo.Authentication.Cache; 

public class InMemoryAuthenticationCacheTests {
    [Fact]
    public async Task Get_value_returns_null_if_no_value_is_set() {
        // Arrange
        var sut = Sut();
        
        // Act
        var result = await sut.Get();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Get_value_returns_value_if_value_is_set() {
        // Arrange
        var sut = Sut();
        var value = CreateAuthenticationDto();
        await sut.Persist(value);
        
        // Act
        var result = await sut.Get();

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task Clear_removes_cached_value() {
        // Arrange
        var sut = Sut();
        var value = CreateAuthenticationDto();
        await sut.Persist(value);
        
        // Act
        await sut.Clear();
        var result = await sut.Get();

        // Assert
        result.Should().BeNull();
    }

    private AuthenticationDto CreateAuthenticationDto() {
        return new AuthenticationDto {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            UserId = "user-id",
            CreatedOnUtc = 12.January(2023),
            AccessTokenTtlSeconds = 1000
        };
    }

    private static InMemoryAuthenticationCache Sut() {
        return new InMemoryAuthenticationCache();
    }
}