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
    using SuperScrabble.Services.Game.Validation;

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

        [Test]
        public void WriteWord_ValidInput_Should_ReturnCorrectResult()
        {
        }

        //Tests for:
        //First player not placing on the center or placing 1 tile

        //TODO: probably add isVertical parameter
        [Test]
        [TestCase(7, 2, 3)]
        [TestCase(7, 4, 7)]
        [TestCase(7, 10, 1)]
        [TestCase(7, 10, 2)]
        public void WriteWord_BoardCellAlreadyTakenAtAnyOfTheInputTilesPositions_Should_ReturnCorrectResult(int startingRow, int startingCol, int tilesCount)
        {
            var gameplayConstantsProvider = new StandardGameplayConstantsProvider();

            var gameService = new GameService(
                new FakeShuffleService(),
                new FakeTilesProvider(gameplayConstantsProvider),
                new MyOldBoardBonusCellsProvider(),
                gameplayConstantsProvider,
                new FakeScoringService(),
                new GameValidator(gameplayConstantsProvider,
                new MyOldBoardTilesProvider(gameplayConstantsProvider),
                new AlwaysValidWordsService()));

            GameState gameState = gameService.CreateGame(this.twoValidConnectionIdsByUserNames);

            gameService.FillPlayerTiles(gameState, gameState.CurrentPlayer.UserName);

            var tilesPlacedOnBoard = new List<Tile>();

            for (int i = 0; i < 7; i++)
            {
                var tempTile = gameState.TilesBag.DrawTile();
                tilesPlacedOnBoard.Add(tempTile);
                gameState.Board[7, 4 + i].Tile = tempTile;
            }

            var invalidInputTiles = new List<KeyValuePair<Tile, Position>>();

            for (int i = 0; i < tilesCount; i++)
            {
                invalidInputTiles.Add(new(gameState.CurrentPlayer.GetTile(i), new Position(startingRow, startingCol + i)));
            }

            var input = new WriteWordInputModel
            {
                PositionsByTiles = invalidInputTiles
            };

            GameOperationResult result = gameService.WriteWord(gameState, input, gameState.CurrentPlayer.UserName);

            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual(1, result.ErrorsByCodes.Count);
            Assert.AreEqual(nameof(Resource.TilePositionAlreadyTaken), result.ErrorsByCodes.First().Key);

            for (int i = 0; i < tilesPlacedOnBoard.Count; i++)
            {
                Assert.AreEqual(gameState.Board[7, 4 + i].Tile, tilesPlacedOnBoard[i]);
            }
        }

        //TODO: pass invalid positions
        [Test]
        //public void WriteWord_InputTilesPositionsOutsideTheBoardRange_Should_ReturnCorrectResult(params Position[] invalidPositions)
        public void WriteWord_InputTilesPositionsOutsideTheBoardRange_Should_ReturnCorrectResult()
        {
            var invalidPositions = new Position[4];
            invalidPositions[0] = new Position(-1, 2);
            invalidPositions[0] = new Position(-1, -2);
            invalidPositions[0] = new Position(50, -2);
            invalidPositions[0] = new Position(230, 2);

            var gameplayConstantsProvider = new StandardGameplayConstantsProvider();

            var gameService = new GameService(
                new FakeShuffleService(),
                new FakeTilesProvider(gameplayConstantsProvider),
                new MyOldBoardBonusCellsProvider(),
                gameplayConstantsProvider,
                new FakeScoringService(),
                new GameValidator(gameplayConstantsProvider,
                new MyOldBoardTilesProvider(gameplayConstantsProvider),
                new AlwaysValidWordsService()));

            GameState gameState = gameService.CreateGame(this.twoValidConnectionIdsByUserNames);

            gameService.FillPlayerTiles(gameState, gameState.CurrentPlayer.UserName);

            var invalidInputTiles = new List<KeyValuePair<Tile, Position>>();

            for (int i = 0; i < invalidPositions.Count(); i++)
            {
                invalidInputTiles.Add(new(gameState.CurrentPlayer.GetTile(i), invalidPositions[i]));
            }

            var input = new WriteWordInputModel
            {
                PositionsByTiles = invalidInputTiles
            };

            GameOperationResult result = gameService.WriteWord(gameState, input, gameState.CurrentPlayer.UserName);

            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual(1, result.ErrorsByCodes.Count);
            Assert.AreEqual(nameof(Resource.TilePositionOutsideBoardRange), result.ErrorsByCodes.First().Key);
        }

        [Test]
        public void WriteWord_InputTilesOnDuplicatePositions_Should_ReturnCorrectResult()
        {
            var gameplayConstantsProvider = new StandardGameplayConstantsProvider();

            var gameService = new GameService(
                new FakeShuffleService(),
                new FakeTilesProvider(gameplayConstantsProvider),
                new MyOldBoardBonusCellsProvider(),
                gameplayConstantsProvider,
                new FakeScoringService(),
                new GameValidator(gameplayConstantsProvider,
                new MyOldBoardTilesProvider(gameplayConstantsProvider),
                new AlwaysValidWordsService()));

            GameState gameState = gameService.CreateGame(this.twoValidConnectionIdsByUserNames);

            gameService.FillPlayerTiles(gameState, gameState.CurrentPlayer.UserName);

            var invalidInputTiles = new List<KeyValuePair<Tile, Position>>();

            for (int i = 0; i < 3; i++)
            {
                invalidInputTiles.Add(new(gameState.CurrentPlayer.GetTile(i), new Position(0, i)));
            }

            invalidInputTiles.Add(new(gameState.CurrentPlayer.GetTile(3), new Position(0, 1)));

            var input = new WriteWordInputModel
            {
                PositionsByTiles = invalidInputTiles
            };

            GameOperationResult result = gameService.WriteWord(gameState, input, gameState.CurrentPlayer.UserName);

            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual(1, result.ErrorsByCodes.Count);
            Assert.AreEqual(nameof(Resource.InputTilesPositionsCollide), result.ErrorsByCodes.First().Key);
        }


        //TODO: Probably needs more TestCases
        [Test]
        public void WriteWord_InputTilesAreNotPlacedOnTheSameLine_Should_ReturnCorrectResult()
        {
            var gameplayConstantsProvider = new StandardGameplayConstantsProvider();

            var gameService = new GameService(
                new FakeShuffleService(),
                new FakeTilesProvider(gameplayConstantsProvider),
                new MyOldBoardBonusCellsProvider(),
                gameplayConstantsProvider,
                new FakeScoringService(),
                new GameValidator(gameplayConstantsProvider,
                new MyOldBoardTilesProvider(gameplayConstantsProvider),
                new AlwaysValidWordsService()));

            GameState gameState = gameService.CreateGame(this.twoValidConnectionIdsByUserNames);

            gameService.FillPlayerTiles(gameState, gameState.CurrentPlayer.UserName);

            var invalidInputTiles = new List<KeyValuePair<Tile, Position>>();

            for (int i = 0; i < 3; i++)
            {
                invalidInputTiles.Add(new(gameState.CurrentPlayer.GetTile(i), new Position(0, i)));
            }

            for (int i = 0; i < 3; i++)
            {
                invalidInputTiles.Add(new(gameState.CurrentPlayer.GetTile(3 + i), new Position(1 + i, 0)));
            }

            var input = new WriteWordInputModel
            {
                PositionsByTiles = invalidInputTiles
            };

            GameOperationResult result = gameService.WriteWord(gameState, input, gameState.CurrentPlayer.UserName);

            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual(1, result.ErrorsByCodes.Count);
            Assert.AreEqual(nameof(Resource.TilesNotOnTheSameLine), result.ErrorsByCodes.First().Key);
        }

        //TODO: Add TestCases and remove the "magic" numbers
        [Test]
        public void WriteWord_InputTilesWithGapsBetweenThePositions_Should_ReturnCorrectResult()
        {
            var gameplayConstantsProvider = new StandardGameplayConstantsProvider();

            var gameService = new GameService(
                new FakeShuffleService(),
                new FakeTilesProvider(gameplayConstantsProvider),
                new MyOldBoardBonusCellsProvider(),
                gameplayConstantsProvider,
                new FakeScoringService(),
                new GameValidator(gameplayConstantsProvider,
                new MyOldBoardTilesProvider(gameplayConstantsProvider),
                new AlwaysValidWordsService()));

            GameState gameState = gameService.CreateGame(this.twoValidConnectionIdsByUserNames);

            gameService.FillPlayerTiles(gameState, gameState.CurrentPlayer.UserName);

            var invalidInputTiles = new List<KeyValuePair<Tile, Position>>();

            for (int i = 0; i < 3; i++)
            {
                invalidInputTiles.Add(new(gameState.CurrentPlayer.GetTile(i), new Position(7, 6 + i)));
            }

            for (int i = 0; i < 2; i++)
            {
                invalidInputTiles.Add(new(gameState.CurrentPlayer.GetTile(3 + i), new Position(7, 10 + i)));
            }

            var input = new WriteWordInputModel
            {
                PositionsByTiles = invalidInputTiles
            };

            GameOperationResult result = gameService.WriteWord(gameState, input, gameState.CurrentPlayer.UserName);

            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual(1, result.ErrorsByCodes.Count);
            Assert.AreEqual(nameof(Resource.GapsBetweenInputTilesNotAllowed), result.ErrorsByCodes.First().Key);
        }

        //Pass incorrect character (e.x Chinese symbol, English one etc)
        //Pass empty wildcard
        [Test]
        public void WriteWord_InvalidInputTilesWildcardValue_Should_ReturnCorrectResult()
        {
            //Verify that the user has a wildcard
        }

        [Test]
        public void WriteWord_TilesWhichUserDoesNotOwn_Should_ReturnCorrectResult()
        {
            var gameplayConstantsProvider = new StandardGameplayConstantsProvider();

            var gameService = new GameService(
                new FakeShuffleService(),
                new FakeTilesProvider(gameplayConstantsProvider),
                new MyOldBoardBonusCellsProvider(),
                gameplayConstantsProvider,
                new FakeScoringService(),
                new GameValidator(gameplayConstantsProvider, new MyOldBoardTilesProvider(gameplayConstantsProvider), new AlwaysValidWordsService()));

            GameState gameState = gameService.CreateGame(this.twoValidConnectionIdsByUserNames);

            gameService.FillPlayerTiles(gameState, gameState.CurrentPlayer.UserName);

            var invalidInputTiles = new List<KeyValuePair<Tile, Position>>();

            for (int i = 0; i < 3; i++)
            {
                invalidInputTiles.Add(new(gameState.CurrentPlayer.GetTile(i), new Position(0, i)));
            }

            invalidInputTiles.Add(new(gameState.TilesBag.DrawTile(), new Position(0, 3)));

            var input = new WriteWordInputModel
            {
                PositionsByTiles = invalidInputTiles
            };

            GameOperationResult result = gameService.WriteWord(gameState, input, gameState.CurrentPlayer.UserName);

            Assert.IsFalse(result.IsSucceeded);
            Assert.AreEqual(1, result.ErrorsByCodes.Count);
            Assert.AreEqual(nameof(Resource.UnexistingPlayerTile), result.ErrorsByCodes.First().Key);
        }

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
                new FakeScoringService(),
                new GameValidator(gameplayConstantsProvider, new MyOldBoardTilesProvider(gameplayConstantsProvider), new AlwaysValidWordsService()));

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
                new FakeTilesProvider(gameplayConstantsProvider),
                new MyOldBoardBonusCellsProvider(),
                gameplayConstantsProvider,
                new FakeScoringService(),
                new GameValidator(gameplayConstantsProvider,
                new MyOldBoardTilesProvider(gameplayConstantsProvider),
                new AlwaysValidWordsService()));

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

    public class FakeTilesProvider : BaseTilesProvider
    {
        public FakeTilesProvider(IGameplayConstantsProvider gameplayConstantsProvider)
            : base(gameplayConstantsProvider)
        {
        }

        public override IEnumerable<KeyValuePair<char, TileInfo>> GetTiles()
        {
            return new Dictionary<char, TileInfo>()
            {
                ['А'] = new(1, 1),
                ['Б'] = new(2, 1),
                ['В'] = new(2, 1),
                ['Г'] = new(3, 1),
                ['Д'] = new(2, 1),
                ['Е'] = new(1, 1),
                ['Ж'] = new(4, 1),
                ['З'] = new(4, 1),
                ['И'] = new(1, 1),
                ['Й'] = new(5, 1),
                ['К'] = new(2, 1),
                ['Л'] = new(2, 1),
                ['М'] = new(2, 1),
                ['Н'] = new(1, 1),
                ['О'] = new(1, 1),
                ['П'] = new(1, 1),
                ['Р'] = new(1, 1),
                ['С'] = new(1, 1),
                ['Т'] = new(1, 1),
                ['У'] = new(5, 1),
                ['Ф'] = new(10,11),
                ['Х'] = new(5, 1),
                ['Ц'] = new(8, 1),
                ['Ч'] = new(5, 1),
                ['Ш'] = new(8, 1),
                ['Щ'] = new(10, 1),
                ['Ъ'] = new(3, 1),
                ['Ь'] = new(10, 1),
                ['Ю'] = new(8, 1),
                ['Я'] = new(5, 1),
                [this.gameplayConstantsProvider.WildcardValue] = new(0, 1),
            };
        }
    }
}
