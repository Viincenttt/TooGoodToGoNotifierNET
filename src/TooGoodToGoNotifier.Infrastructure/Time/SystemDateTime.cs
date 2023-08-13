using TooGoodToGoNotifier.Application.Common.Interfaces;

namespace TooGoodToGoNotifier.Infrastructure.Time; 

public class SystemDateTime : IDateTimeProvider {
    public DateTime UtcNow() {
        return DateTime.UtcNow;
    }
}