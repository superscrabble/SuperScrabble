using SuperScrabble.Common;
using SuperScrabble.Common.Exceptions.Game;
using SuperScrabble.Services.Data.Words;
using SuperScrabble.Services.Game.Common;
using SuperScrabble.Services.Game.Common.GameplayConstantsProviders;
using SuperScrabble.Services.Game.Common.TilesProviders;
using SuperScrabble.Services.Game.Models;
using SuperScrabble.Services.Game.Validation;
using System.Linq;
using Xunit;

namespace SuperScrabble.Services.Game.Tests;

public class SubmittedTilesOwnershipTests
{
    private static GameValidator CreateValidator()
    {
        return new GameValidator(
            new StandardGameplayConstants(playersPerGameCount: 2, gameTimerSeconds: 90),
            new StandardTilesProvider(),
            new AlwaysValidWordsService());
    }

    [Fact]
    public void HasPlayerSubmittedTilesWhichHeOwns_SingleWildcardPlayedAsValidLetter_Should_NotThrow()
    {
        var player = new Player("player", "connection-id");
        player.AddTile(new Tile(GlobalConstants.WildcardValue, 0));

        var wildcardOption = new StandardTilesProvider().GetAllWildcardOptions().First();
        var submitted = new[] { new Tile(wildcardOption.Letter, 0) };

        CreateValidator().HasPlayerSubmittedTilesWhichHeOwns(
            player, submitted, isPlayerTryingToExchangeTiles: false);
    }

    [Fact]
    public void HasPlayerSubmittedTilesWhichHeOwns_TwoWildcardPlaysWithOneWildcardOwned_Should_Throw()
    {
        var player = new Player("player", "connection-id");
        player.AddTile(new Tile(GlobalConstants.WildcardValue, 0));

        var options = new StandardTilesProvider().GetAllWildcardOptions().Take(2).ToList();
        var submitted = new[]
        {
            new Tile(options[0].Letter, 0),
            new Tile(options[1].Letter, 0),
        };

        Assert.Throws<UnexistingPlayerTilesException>(() =>
            CreateValidator().HasPlayerSubmittedTilesWhichHeOwns(
                player, submitted, isPlayerTryingToExchangeTiles: false));
    }

    [Fact]
    public void HasPlayerSubmittedTilesWhichHeOwns_SameRegularTileSubmittedTwiceWhileOwningOne_Should_Throw()
    {
        var player = new Player("player", "connection-id");
        player.AddTile(new Tile('А', 1));

        var submitted = new[]
        {
            new Tile('А', 1),
            new Tile('А', 1),
        };

        Assert.Throws<UnexistingPlayerTilesException>(() =>
            CreateValidator().HasPlayerSubmittedTilesWhichHeOwns(
                player, submitted, isPlayerTryingToExchangeTiles: false));
    }
}
