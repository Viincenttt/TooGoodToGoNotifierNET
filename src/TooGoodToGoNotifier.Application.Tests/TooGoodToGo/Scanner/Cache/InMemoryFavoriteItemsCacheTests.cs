using FluentAssertions;
using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner;
using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner.Cache;

namespace TooGoodToGoNotifier.Application.Tests.TooGoodToGo.Scanner.Cache; 

public class InMemoryFavoriteItemsCacheTests {
    [Fact]
    public async Task Get_value_returns_empty_dictionary_if_no_value_is_set() {
        // Arrange
        var sut = Sut();
        
        // Act
        var result = await sut.Get();

        // Assert
        result.Should().HaveCount(0);
    }
    
    [Fact]
    public async Task Persisted_values_can_be_retrieved() {
        // Arrange
        var sut = Sut();
        const int numberOfItemsToGenerate = 10;
        var values = Enumerable.Range(1, numberOfItemsToGenerate)
            .ToDictionary(x => $"item-id-{x}", x => new FavoriteItemDto {
                ItemId = $"item-id-{x}",
                DisplayName = $"display-name-{x}",
                ItemsAvailable = x
            });
        
        // Act
        await sut.Persist(values);
        var result = await sut.Get();

        // Assert
        result.Should().HaveCount(numberOfItemsToGenerate);
        foreach (var keyValuePair in result) {
            values.Should().ContainKey(keyValuePair.Key);
            values.Should().ContainValue(keyValuePair.Value);
        }
    }

    private static InMemoryFavoriteItemsCache Sut() {
        return new InMemoryFavoriteItemsCache();
    }
}