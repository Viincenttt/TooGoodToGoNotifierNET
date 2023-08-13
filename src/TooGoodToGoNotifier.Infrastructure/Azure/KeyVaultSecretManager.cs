using System.Net;
using Azure;
using Azure.Security.KeyVault.Secrets;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Domain.Secrets;

namespace TooGoodToGoNotifier.Infrastructure.Azure; 

public class KeyVaultSecretManager : ISecretsManager {
    private readonly SecretClient _secretClient;
    private readonly IDateTimeProvider _dateTimeProvider;

    public KeyVaultSecretManager(SecretClient secretClient, IDateTimeProvider dateTimeProvider) {
        _secretClient = secretClient;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<SecretDto?> GetSecret(string name) {
        try {
            var secret = await _secretClient.GetSecretAsync(name);
            bool isValid = secret.Value.Properties.ExpiresOn > _dateTimeProvider.UtcNow();
            if (isValid) {
                return new SecretDto {
                    Name = name,
                    Value = secret.Value.Value,
                    CreatedOn = secret.Value.Properties.CreatedOn?.DateTime.ToUniversalTime(),
                    ExpiresOn = secret.Value.Properties.ExpiresOn?.DateTime.ToUniversalTime()
                };
            }

            return null;
        }
        catch(RequestFailedException e) {
            if (e.Status == (int)HttpStatusCode.NotFound) {
                return null;
            }

            throw;
        }
    }

    public async Task SetSecret(string name, string value, DateTime? expiresOn = null) {
        var secret = new KeyVaultSecret(name, value);
        if (expiresOn.HasValue) {
            secret.Properties.ExpiresOn = expiresOn;
        }
        await _secretClient.SetSecretAsync(secret);
    }

    public async Task RemoveSecret(string name) {
        await _secretClient.StartDeleteSecretAsync(name);
    }
}