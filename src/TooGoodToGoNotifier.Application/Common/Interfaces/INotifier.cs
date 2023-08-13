using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner;

namespace TooGoodToGoNotifier.Application.Common.Interfaces; 

public interface INotifier {
    Task Notify(FavoriteItemDto favoriteItemDto);
}