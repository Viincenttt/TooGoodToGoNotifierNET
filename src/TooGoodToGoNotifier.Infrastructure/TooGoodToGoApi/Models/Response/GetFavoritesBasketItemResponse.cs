namespace TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Response;

public record GetFavoritesBasketItemResponse {
    public required string ItemId { get; init; }
    public required string DisplayName { get; init; }
    public required int ItemsAvailable { get; init; }
}