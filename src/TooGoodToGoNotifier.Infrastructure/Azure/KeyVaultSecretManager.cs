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
            bool isValid = !secret.Value.Properties.ExpiresOn.HasValue || secret.Value.Properties.ExpiresOn > _dateTimeProvider.UtcNow();
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
        // We don't actually remove the secrets, because some keyvault have soft-delete / purge protection enabled
        // Therefore, we just update the expires on, so the secret is invalidated.
        try {
            var secret = await _secretClient.GetSecretAsync(name);
            secret.Value.Properties.ExpiresOn = _dateTimeProvider.UtcNow();
            await _secretClient.UpdateSecretPropertiesAsync(secret.Value.Properties);
        }
        catch (RequestFailedException e) {
            if (e.Status != (int)HttpStatusCode.NotFound) {
                throw;
            }
        }
    }
}