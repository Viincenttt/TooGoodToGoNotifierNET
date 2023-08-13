namespace TooGoodToGoNotifier.Application.Common.Interfaces; 

public interface IDateTimeProvider {
    public DateTime UtcNow();
}