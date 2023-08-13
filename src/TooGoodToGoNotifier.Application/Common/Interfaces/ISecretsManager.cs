using TooGoodToGoNotifier.Domain.Secrets;

namespace TooGoodToGoNotifier.Application.Common.Interfaces; 

public interface ISecretsManager {
    Task<SecretDto?> GetSecret(string name);
    Task SetSecret(string name, string value, DateTime? expiresOn = null);
    Task RemoveSecret(string name);
}