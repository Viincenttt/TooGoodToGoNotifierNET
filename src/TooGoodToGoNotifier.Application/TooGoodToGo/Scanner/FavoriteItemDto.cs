namespace TooGoodToGoNotifier.Application.TooGoodToGo.Scanner; 

public record FavoriteItemDto {
    public required string ItemId { get; init; }
    public required string DisplayName { get; init; }
    public required int ItemsAvailable { get; init; }
}