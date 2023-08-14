using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication;
using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication.Cache;
using TooGoodToGoNotifier.Domain.Secrets;

namespace TooGoodToGoNotifier.Application.Tests.TooGoodToGo.Authentication.Cache; 

public class SecretsManagerAuthenticationCacheTests {
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
        var value = CreateAuthenticationDto();
        var secretsManager = new Mock<ISecretsManager>();
        secretsManager.Setup(x => x.GetSecret("Tgtg--UserId"))
            .ReturnsAsync((string key) => CreateSecretDto(key, value.UserId));
        secretsManager.Setup(x => x.GetSecret("Tgtg--AccessToken"))
            .ReturnsAsync((string key) => CreateSecretDto(key, value.AccessToken));
        secretsManager.Setup(x => x.GetSecret("Tgtg--RefreshToken"))
            .ReturnsAsync((string key) => CreateSecretDto(key, value.RefreshToken));
        var sut = Sut(secretsManager);
        
        // Act
        var result = await sut.Get();

        // Assert
        result.Should().BeEquivalentTo(value);
    }
    
    [Fact]
    public async Task Persist_calls_set_secret_method_on_secretsmanager() {
        // Arrange
        var value = CreateAuthenticationDto();
        var secretsManager = new Mock<ISecretsManager>();
        var sut = Sut(secretsManager);
        
        // Act
        await sut.Persist(value);

        // Assert
        secretsManager.Verify(x => x.SetSecret("Tgtg--UserId", value.UserId, null));
        secretsManager.Verify(x => x.SetSecret("Tgtg--AccessToken", value.AccessToken, value.ValidUntilUtc));
        secretsManager.Verify(x => x.SetSecret("Tgtg--RefreshToken", value.RefreshToken, null));
    }
    
    [Fact]
    public async Task Clear_removes_cached_value() {
        // Arrange
        var secretsManager = new Mock<ISecretsManager>();
        var sut = Sut(secretsManager);
        
        // Act
        await sut.Clear();

        // Assert
        secretsManager.Verify(x => x.RemoveSecret("Tgtg--UserId"));
        secretsManager.Verify(x => x.RemoveSecret("Tgtg--AccessToken"));
        secretsManager.Verify(x => x.RemoveSecret("Tgtg--RefreshToken"));
    }

    private SecretDto CreateSecretDto(string name, string value) {
        return new SecretDto {
            Name = name,
            Value = value,
            CreatedOn = 12.January(2023),
            ExpiresOn = 12.January(2023).AddSeconds(1000)
        };
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

    private SecretsManagerAuthenticationCache Sut(Mock<ISecretsManager> secretsManager = null) {
        secretsManager ??= new Mock<ISecretsManager>();
        return new SecretsManagerAuthenticationCache(secretsManager.Object);
    }
}