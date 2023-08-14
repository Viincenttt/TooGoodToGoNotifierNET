using FluentAssertions.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication;
using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner;
using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner.Cache;
using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Request;
using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Response;

namespace TooGoodToGoNotifier.Application.Tests.TooGoodToGo.Scanner; 

public class FavoritesScannerTests {
    [Fact]
    public async Task When_no_new_favorite_item_is_detected_no_notification_is_sent() {
        // Arrange
        var tgtgApiClient = CreateApiClient(new FavoritesItemsResponse {
            Items = new List<FavoritesItemsResponse.ItemResponse> {
                CreateFavoritesItem("item-id", "display-name", 1)
            }
        });
        var favoritesCache = CreateCache(new Dictionary<string, FavoriteItemDto>() {
            {
                "item-id", new FavoriteItemDto {
                    ItemId = "item-id",
                    DisplayName = "display-name",
                    ItemsAvailable = 1
                }
            }
        });
        var notifier = new Mock<INotifier>();
        var sut = Sut(tgtgApiClient: tgtgApiClient, cache: favoritesCache, notifier: notifier);

        // Act
        await sut.ScanFavorites("my-email", CancellationToken.None);

        // Assert
        notifier.Verify(x => x.Notify(It.IsAny<FavoriteItemDto>()), Times.Never);
        tgtgApiClient.Verify(x => 
            x.GetFavoritesItems(It.IsAny<string>(), It.IsAny<FavoritesItemsRequest>()), Times.Once);
        favoritesCache.Verify(x => x.Get(), Times.Once);
    }
    
    [Fact]
    public async Task Send_notification_when_new_favorite_is_detected() {
        // Arrange
        var tgtgApiClient = CreateApiClient(new FavoritesItemsResponse {
            Items = new List<FavoritesItemsResponse.ItemResponse> {
                CreateFavoritesItem("item-id", "display-name", 1)
            }
        });
        var favoritesCache = CreateCache(new Dictionary<string, FavoriteItemDto>());
        var notifier = new Mock<INotifier>();
        var sut = Sut(tgtgApiClient: tgtgApiClient, cache: favoritesCache, notifier: notifier);

        // Act
        await sut.ScanFavorites("my-email", CancellationToken.None);

        // Assert
        notifier.Verify(x => 
            x.Notify(It.Is<FavoriteItemDto>(item => item.ItemId == "item-id")), Times.Once);
        tgtgApiClient.Verify(x => 
            x.GetFavoritesItems(It.IsAny<string>(), It.IsAny<FavoritesItemsRequest>()), Times.Once);
        favoritesCache.Verify(x => x.Get(), Times.Once);
    }

    private static FavoritesItemsResponse.ItemResponse CreateFavoritesItem(
        string itemId, 
        string displayName,
        int itemsAvailable) {

        return new FavoritesItemsResponse.ItemResponse {
            Item = new FavoritesItemsResponse.ItemResponse.ItemDetails {
                ItemId = itemId
            },
            DisplayName = displayName,
            ItemsAvailable = itemsAvailable
        };
    }

    private static Mock<ITooGoodToGoApiClient> CreateApiClient(FavoritesItemsResponse items) {
        var api = new Mock<ITooGoodToGoApiClient>();
        api.Setup(x => 
                x.GetFavoritesItems(It.IsAny<string>(), It.IsAny<FavoritesItemsRequest>()))
            .ReturnsAsync(items);
        return api;
    }

    private static Mock<IFavoriteItemsCache> CreateCache(Dictionary<string, FavoriteItemDto> items) {
        var cache = new Mock<IFavoriteItemsCache>();
        cache.Setup(x => x.Get())
            .ReturnsAsync(items);
        return cache;
    }

    private static Mock<ITooGoodToGoAuthenticator> CreateAuthenticator() {
        const string email = "my-email";
        var authenticator = new Mock<ITooGoodToGoAuthenticator>();
        authenticator.Setup(x => x.Authenticate(email, CancellationToken.None))
            .ReturnsAsync(new AuthenticationDto {
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                UserId = "user-id",
                CreatedOnUtc = 14.August(2023),
                AccessTokenTtlSeconds = 1000
            });
        return authenticator;
    }

    private static FavoritesScanner Sut(
        Mock<ITooGoodToGoApiClient>? tgtgApiClient = null,
        Mock<IFavoriteItemsCache>? cache = null,
        Mock<INotifier>? notifier = null,
        Mock<ILogger<FavoritesScanner>>? logger = null,
        Mock<ITooGoodToGoAuthenticator>? authenticator = null) {

        authenticator ??= CreateAuthenticator();
        tgtgApiClient ??= CreateApiClient(new FavoritesItemsResponse {
            Items = Enumerable.Empty<FavoritesItemsResponse.ItemResponse>()
        });
        cache ??= CreateCache(new Dictionary<string, FavoriteItemDto>());
        notifier ??= new Mock<INotifier>();
        logger ??= new Mock<ILogger<FavoritesScanner>>();

        return new FavoritesScanner(
            authenticator.Object,
            tgtgApiClient.Object,
            cache.Object,
            new[] { notifier.Object },
            logger.Object);
    }
}