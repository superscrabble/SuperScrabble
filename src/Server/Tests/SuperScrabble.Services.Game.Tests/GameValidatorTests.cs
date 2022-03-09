using SuperScrabble.Common;
using SuperScrabble.Common.Exceptions.Game;
using SuperScrabble.Services.Data.Words;
using SuperScrabble.Services.Game.Common;
using SuperScrabble.Services.Game.Common.BonusCellsProviders;
using SuperScrabble.Services.Game.Common.GameplayConstantsProviders;
using SuperScrabble.Services.Game.Common.TilesProviders;
using SuperScrabble.Services.Game.Models;
using SuperScrabble.Services.Game.Models.Bags;
using SuperScrabble.Services.Game.Models.Boards;
using SuperScrabble.Services.Game.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SuperScrabble.Services.Game.Tests;

public class GameValidatorTests
{
    [Fact]
    public void IsPlayerOnTurn_ValidInput_Should_NotThrowException()
    {
        try
        {
            var gameState = new FakeGameState();
            new GameValidator(
                gameState.GameplayConstants,
                new StandardTilesProvider(),
                new AlwaysValidWordsService()).IsPlayerOnTurn(gameState, "Denis");
        }
        catch (Exception)
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void IsPlayerOnTurn_InvalidInput_Should_ThrowException()
    {
        var gameState = new FakeGameState();
        Assert.Throws<PlayerNotOnTurnException>(() =>
        {
            new GameValidator(
                gameState.GameplayConstants,
                new StandardTilesProvider(),
                new AlwaysValidWordsService()).IsPlayerOnTurn(gameState, "Georgi");
        });
    }

    [Theory]
    [InlineData(-5, true)]
    [InlineData(0, true)]
    [InlineData(9, true)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void ValidateInputTilesCount_InvalidInput_Should_ThrowException(int inputTilesCount, bool isBoardEmpty)
    {
        var gameState = new FakeGameState();
        int playerTilesCount = gameState.CurrentTeam.CurrentPlayer.Tiles.Count;
        Assert.Throws<InvalidInputTilesCountException>(() =>
        {
            new GameValidator(
                gameState.GameplayConstants,
                new StandardTilesProvider(),
                new AlwaysValidWordsService())
            .ValidateInputTilesCount(playerTilesCount, inputTilesCount, isBoardEmpty);
        });
    }

    [Theory]
    [InlineData(3, true)]
    [InlineData(1, false)]
    [InlineData(5, false)]
    public void ValidateInputTilesCount_ValidInput_Should_NotThrowException(int inputTilesCount, bool isBoardEmpty)
    {
        try
        {
            var gameState = new FakeGameState();
            int playerTilesCount = gameState.CurrentTeam.CurrentPlayer.Tiles.Count;
            new GameValidator(
                gameState.GameplayConstants,
                new StandardTilesProvider(),
                new AlwaysValidWordsService())
            .ValidateInputTilesCount(playerTilesCount, inputTilesCount, isBoardEmpty);
        }
        catch (Exception)
        {
            Assert.True(false);
        }
    }

    [Theory]
    [InlineData("0,0", "2,3", "1,8", "7,9")]
    [InlineData("4,4", "9,9", "2,0", "14,14")]
    [InlineData("1,1", "0,0", "2,0", "7,9", "13,13")]
    public void AreAllTilesInsideTheBoardRange_ValidInput_Should_NotThrowException(params string[] tilePositionsAsText)
    {
        var positions = tilePositionsAsText.Select(x =>
        {
            var args = x.Split(",");
            var row = int.Parse(args[0]);
            var column = int.Parse(args[1]);
            return new Position(row, column);
        });

        var gameState = new FakeGameState();

        try
        {
            new GameValidator(
                gameState.GameplayConstants,
                new StandardTilesProvider(),
                new AlwaysValidWordsService())
            .AreAllTilesInsideTheBoardRange(gameState.Board, positions);
        }
        catch (Exception)
        {
            Assert.True(false);
        }
    }

    [Theory]
    [InlineData("0,-9", "2,3", "1,8", "7,9")]
    [InlineData("4,4", "9,9", "2,0", "15,14")]
    [InlineData("1,1", "15,15", "2,0", "7,9", "13,13")]
    public void AreAllTilesInsideTheBoardRange_InvalidInput_Should_ThrowException(params string[] tilePositionsAsText)
    {
        var positions = tilePositionsAsText.Select(x =>
        {
            var args = x.Split(",");
            var row = int.Parse(args[0]);
            var column = int.Parse(args[1]);
            return new Position(row, column);
        });

        var gameState = new FakeGameState();

        Assert.Throws<TilePositionOutsideBoardRangeException>(() =>
            new GameValidator(
                gameState.GameplayConstants,
                new StandardTilesProvider(),
                new AlwaysValidWordsService())
            .AreAllTilesInsideTheBoardRange(gameState.Board, positions));
    }

    [Theory]
    [InlineData("0,0", "2,3", "1,8", "7,9")]
    [InlineData("4,4", "9,9", "2,0", "14,14")]
    [InlineData("1,1", "0,0", "2,0", "7,9", "13,13")]
    public void AreAllTilesPositionsFreeBoardCells_ValidInput_Should_NotThrowException(params string[] tilePositionsAsText)
    {
        var positions = tilePositionsAsText.Select(x =>
        {
            var args = x.Split(",");
            var row = int.Parse(args[0]);
            var column = int.Parse(args[1]);
            return new Position(row, column);
        });

        var gameState = new FakeGameState();
        gameState.Board[3, 3].Tile = new Tile('Г', 3);
        gameState.Board[5, 5].Tile = new Tile('Г', 3);
        gameState.Board[6, 6].Tile = new Tile('Г', 3);
        gameState.Board[7, 7].Tile = new Tile('Г', 3);

        try
        {
            new GameValidator(
                gameState.GameplayConstants,
                new StandardTilesProvider(),
                new AlwaysValidWordsService())
            .AreAllTilesPositionsFreeBoardCells(gameState.Board, positions);
        }
        catch (Exception)
        {
            Assert.True(false);
        }
    }

    [Theory]
    [InlineData("3,3", "2,3", "1,8", "7,9")]
    [InlineData("4,4", "5,5", "2,0", "14,14")]
    [InlineData("1,1", "0,0", "7,7", "7,9", "13,13")]
    public void AreAllTilesPositionsFreeBoardCells_InvalidInput_Should_NotThrowException(params string[] tilePositionsAsText)
    {
        var positions = tilePositionsAsText.Select(x =>
        {
            var args = x.Split(",");
            var row = int.Parse(args[0]);
            var column = int.Parse(args[1]);
            return new Position(row, column);
        });

        var gameState = new FakeGameState();
        gameState.Board[3, 3].Tile = new Tile('Г', 3);
        gameState.Board[5, 5].Tile = new Tile('Г', 3);
        gameState.Board[6, 6].Tile = new Tile('Г', 3);
        gameState.Board[7, 7].Tile = new Tile('Г', 3);

        Assert.Throws<TilePositionAlreadyTakenException>(() =>
        {
            new GameValidator(
                gameState.GameplayConstants,
                new StandardTilesProvider(),
                new AlwaysValidWordsService())
            .AreAllTilesPositionsFreeBoardCells(gameState.Board, positions);
        });
    }

    [Theory]
    [InlineData(true, new[] { "3,3", "2,3", "1,3", "0,3" })]
    [InlineData(true, new[] { "0,0", "1,0", "2,0", "3,0" })]
    [InlineData(false, new[] { "1,1", "1,2", "1,3", "1,4", "1,5" })]
    public void AreTilesOnTheSameLine_ValidInput_Should_NotThrowException(
        bool expectedAreTilesAllignedVertically, string[] rawPositionsAsText)
    {
        var positions = rawPositionsAsText.Select(x =>
        {
            var args = x.Split(",");
            var row = int.Parse(args[0]);
            var column = int.Parse(args[1]);
            return new Position(row, column);
        });

        var gameState = new FakeGameState();

        try
        {
            new GameValidator(
                gameState.GameplayConstants,
                new StandardTilesProvider(),
                new AlwaysValidWordsService())
            .AreTilesOnTheSameLine(positions, out bool actualareTilesAllignedVertically);

            Assert.Equal(expectedAreTilesAllignedVertically, actualareTilesAllignedVertically);
        }
        catch (Exception)
        {
            Assert.True(false);
        }
    }

    [Theory]
    [InlineData("3,3", "2,3", "6,3", "2,2")]
    [InlineData("0,0", "1,0", "2,1", "3,0")]
    [InlineData("5,9", "1,2", "1,3", "1,4", "1,5")]
    public void AreTilesOnTheSameLine_ValidInput_Should_ThrowException(params string[] rawPositionsAsText)
    {
        var positions = rawPositionsAsText.Select(x =>
        {
            var args = x.Split(",");
            var row = int.Parse(args[0]);
            var column = int.Parse(args[1]);
            return new Position(row, column);
        });

        var gameState = new FakeGameState();

        Assert.Throws<TilesNotOnTheSameLineException>(()
            => new GameValidator(
                gameState.GameplayConstants,
                new StandardTilesProvider(),
                new AlwaysValidWordsService())
                .AreTilesOnTheSameLine(positions, out bool areTilesAllignedVertically));
    }

    [Theory]
    [InlineData("3,0", "0,3", "1,3", "5,3")]
    [InlineData("0,0", "7,0", "2,0", "3,5")]
    [InlineData("1,5", "7,5", "3,3", "1,4", "2,5")]
    public void DoInputTilesHaveDuplicatePositions_ValidInput_Should_NotThrowException(params string[] rawPositionsAsText)
    {
        var positions = rawPositionsAsText.Select(x =>
        {
            var args = x.Split(",");
            var row = int.Parse(args[0]);
            var column = int.Parse(args[1]);
            return new Position(row, column);
        });

        var gameState = new FakeGameState();

        try
        {
            new GameValidator(
                gameState.GameplayConstants,
                new StandardTilesProvider(),
                new AlwaysValidWordsService())
            .DoInputTilesHaveDuplicatePositions(positions);
        }
        catch (Exception)
        {
            Assert.True(false);
        }
    }

    [Theory]
    [InlineData("0,3", "0,3", "1,3", "5,3")]
    [InlineData("0,0", "0,0", "2,0", "3,5")]
    [InlineData("1,5", "7,5", "3,3", "1,4", "1,5")]
    [InlineData("3,3", "3,3")]
    public void DoInputTilesHaveDuplicatePositions_ValidInput_Should_ThrowException(params string[] rawPositionsAsText)
    {
        var positions = rawPositionsAsText.Select(x =>
        {
            var args = x.Split(",");
            var row = int.Parse(args[0]);
            var column = int.Parse(args[1]);
            return new Position(row, column);
        });

        var gameState = new FakeGameState();
        Assert.Throws<InputTilesPositionsCollideException>(() =>
        {
            new GameValidator(
                gameState.GameplayConstants,
                new StandardTilesProvider(),
                new AlwaysValidWordsService())
            .DoInputTilesHaveDuplicatePositions(positions);
        });
    }

    class FakeBag : Bag
    {
        public FakeBag()
            : base(new StandardTilesProvider())
        {
        }
    }

    class FakeGameplayConstants : StandardGameplayConstants
    {
        public FakeGameplayConstants()
            : base(2, 90)
        {
        }
    }

    class FakeGameState : GameState
    {
        private static IEnumerable<Team> GetTeams()
        {
            var firstTeam = new Team();
            firstTeam.AddPlayer("Denis", "qwaszx");
            firstTeam.CurrentPlayer.AddTiles(new Tile[]
            {
                new('А', 1),
                new('А', 1),
                new('А', 1),
                new('А', 1),
                new('А', 1),
                new('А', 1),
                new('А', 1),
            });

            var secondTeam = new Team();
            secondTeam.AddPlayer("Georgi", "zxasqw");
            secondTeam.CurrentPlayer.AddTiles(new Tile[]
            {
                new('А', 1),
                new('А', 1),
                new('А', 1),
                new('А', 1),
                new('А', 1),
                new('А', 1),
                new('А', 1),
            });

            var teams = new List<Team>
            {
                firstTeam,
                secondTeam,
            };

            return teams;
        }

        public FakeGameState()
            : base(
                  new FakeBag(),
                  new StandardBoard(new StandardBonusCellsProvider()),
                  "FakeGroupName",
                  GetTeams(),
                  new FakeGameplayConstants())
        {
        }
    }
}
