namespace SuperScrabble.Tests.Services
{
    using System.Linq;
    using System.Collections.Generic;

    using NUnit.Framework;

    using SuperScrabble.Common;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.LanguageResources;
    using SuperScrabble.Services;
    using SuperScrabble.Services.Data.Words;
    using SuperScrabble.Services.Game;
    using SuperScrabble.Services.Game.BonusCellsProviders;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.Scoring;
    using SuperScrabble.Services.Game.TilesProviders;
    using SuperScrabble.ViewModels;
    
    [TestFixture]
    public class GameServiceTests
    {
        public const int PlayerTilesCount = 7;

        private const string FirstUserName = "Denis";
        private const string SecondUserName = "Georgi";
        private const string ThirdUserName = "Ivan";
        private const string FourthUserName = "Alex";

        private readonly List<KeyValuePair<string, string>> twoValidConnectionIdsByUserNames = new()
        {
            new(FirstUserName, "1234567890"),
            new(SecondUserName, "0987654321"),
        };

        // input -> positions by tiles
        // invalid tiles count (0, 100, 8, 6)
        // tiles which the user does not own (wildcards)
        // repeating tile positions (already taken, outside the board)
        // gaps
        // horizontal and vertical allignment

        [TestCase(FirstUserName, SecondUserName)]
        [TestCase(FirstUserName, SecondUserName, ThirdUserName)]
        [TestCase(FirstUserName, SecondUserName, ThirdUserName, FourthUserName)]
        public void WriteWord_InvalidPlayerOnTurn_Should_ReturnCorrectResult(params string[] userNames)
        {
            var gameplayConstantsProvider = new StandardGameplayConstantsProvider();

            var gameService = new GameService(
                new FakeShuffleService(),
                new MyOldBoardTilesProvider(gameplayConstantsProvider),
                new MyOldBoardBonusCellsProvider(),
                gameplayConstantsProvider,
                new AlwaysValidWordsService(),
                new FakeScoringService());

            var connectionIdsByUserNames = userNames.Select(
                name => new KeyValuePair<string, string>(name, $"{name}123456"));

            GameState gameState = gameService.CreateGame(connectionIdsByUserNames);

            var tileA = new Tile('А', 1);
            var tileB = new Tile('Б', 2);
            var tileC = new Tile('В', 2);

            gameState.Board[7, 7].Tile = tileA;
            gameState.Board[7, 8].Tile = tileB;
            gameState.Board[7, 9].Tile = tileC;

            var input = new WriteWordInputModel
            {
                PositionsByTiles = new List<KeyValuePair<Tile, Position>>()
                {
                    new(new Tile('Г', 3), new Position(7, 7)),
                    new(new Tile('Г', 3), new Position(7, 8)),
                    new(new Tile('Г', 3), new Position(7, 9)),
                }
            };

            for (int i = 1; i < connectionIdsByUserNames.Count(); i++)
            {
                GameOperationResult result = gameService.WriteWord(gameState, input, userNames[i]);

                Assert.IsFalse(result.IsSucceeded);
                Assert.AreEqual(1, result.ErrorsByCodes.Count);
                Assert.AreEqual(nameof(Resource.TheGivenPlayerIsNotOnTurn), result.ErrorsByCodes.First().Key);

                Assert.AreEqual(gameState.Board[7, 7].Tile, tileA);
                Assert.AreEqual(gameState.Board[7, 8].Tile, tileB);
                Assert.AreEqual(gameState.Board[7, 9].Tile, tileC);
            }
        }

        [TestCase(0)]
        [TestCase(100)]
        [TestCase(PlayerTilesCount + 1)]
        [TestCase(PlayerTilesCount + 3)]
        public void WriteWord_InvalidInputTilesCount_Should_ReturnCorrectResult(int positionsByTilesCount)
        {
            var gameplayConstantsProvider = new StandardGameplayConstantsProvider();

            var gameService = new GameService(
                new FakeShuffleService(),
                new MyOldBoardTilesProvider(gameplayConstantsProvider),
                new MyOldBoardBonusCellsProvider(),
                gameplayConstantsProvider,
                new AlwaysValidWordsService(),
                new FakeScoringService());

            GameState gameState = gameService.CreateGame(this.twoValidConnectionIdsByUserNames);

            var invalidPositionsByTiles = new List<KeyValuePair<Tile, Position>>();

            for (int i = 0; i < positionsByTilesCount; i++)
            {
                invalidPositionsByTiles.Add(new(new Tile('А', 1), new Position(0, 0)));
            }

            var input = new WriteWordInputModel
            {
                PositionsByTiles = invalidPositionsByTiles
            };

            GameOperationResult result = gameService.WriteWord(gameState, input, FirstUserName);

            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual(1, result.ErrorsByCodes.Count);
            Assert.AreEqual(nameof(Resource.InvalidInputTilesCount), result.ErrorsByCodes.First().Key);
        }
    }

    public class FakeShuffleService : IShuffleService
    {
        public IEnumerable<T> Shuffle<T>(IEnumerable<T> items)
        {
            return items.ToList();
        }
    }

    public class FakeScoringService : IScoringService
    {
        public int CalculatePointsFromPlayerInput(
            WriteWordInputModel input, IBoard board, IEnumerable<WordBuilder> words)
        {
            return 0;
        }
    }
}
