namespace SuperScrabble.Services.Game.Scoring
{
    using System.Linq;
    using System.Collections.Generic;

    using SuperScrabble.InputModels.Game;
    using SuperScrabble.Services.Game.Models;

    public class ScoringService : IScoringService
    {
        public int CalculatePointsFromPlayerInput(WriteWordInputModel input, IBoard board, IEnumerable<WordBuilder> words)
        {
            int totalPoints = 0;

            foreach (WordBuilder word in words)
            {
                int currentWordPoints = 0;
                var wordBonuses = new List<CellType>();

                foreach (var positionByTile in word.PositionsByTiles)
                {
                    Tile tile = positionByTile.Key;
                    int pointsForCurrentTile = tile.Points;

                    bool isTileNew = input.PositionsByTiles.Any(x => x.Value.Equals(positionByTile.Value));

                    if (isTileNew)
                    {
                        CellType cellType = board[positionByTile.Value].Type;

                        if (cellType.IsWordBonus())
                        {
                            wordBonuses.Add(cellType);
                        }

                        if (cellType.IsLetterBonus())
                        {
                            int increasePoints(int tilePoints) => tilePoints * cellType.GetPointsMultiplier();
                            pointsForCurrentTile = increasePoints(pointsForCurrentTile);
                        }
                    }

                    currentWordPoints += pointsForCurrentTile;
                }

                foreach (CellType wordBonus in wordBonuses)
                {
                    int increaseWordPoints(int wordPoints) => wordPoints * wordBonus.GetPointsMultiplier();
                    currentWordPoints = increaseWordPoints(currentWordPoints);
                }

                totalPoints += currentWordPoints;
            }

            return totalPoints;
        }
    }
}
