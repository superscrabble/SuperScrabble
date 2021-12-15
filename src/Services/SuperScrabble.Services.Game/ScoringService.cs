using SuperScrabble.InputModels.Game;
using SuperScrabble.Services.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.Services.Game
{
    public class ScoringService : IScoringService
    {
        public int CalculatePointsFromPlayerInput(WriteWordInputModel input, IBoard board, IEnumerable<WordBuilder> words)
        {
            int totalPoints = 0;

            foreach (var word in words)
            {
                int currentWordPoints = 0;
                var wordBonuses = new List<CellType>();

                foreach (var positionByTile in word.PositionsByTiles)
                {
                    bool isTileNew = input.PositionsByTiles.Any(x => x.Value.Equals(positionByTile.Value));
                    Tile tile = positionByTile.Key;
                    int pointsForCurrentTile = tile.Points;

                    if (isTileNew)
                    {
                        CellType cellType = board[positionByTile.Value].Type;

                        if (cellType.IsWordBonus())
                        {
                            wordBonuses.Add(cellType);
                        }

                        if (cellType.IsLetterBonus())
                        {
                            Func<int, int> increasePoints = cellType == CellType.DoubleLetter
                                ? tilePoints => tilePoints * 2 : tilePoints => tilePoints * 3;

                            pointsForCurrentTile = increasePoints(pointsForCurrentTile);
                        }
                    }

                    currentWordPoints += pointsForCurrentTile;
                }

                foreach (var wordBonus in wordBonuses)
                {
                    Func<int, int> increaseWordPoints = wordBonus == CellType.DoubleWord
                        ? wordPoints => wordPoints * 2 : wordPoints => wordPoints * 3;

                    currentWordPoints = increaseWordPoints(currentWordPoints);
                }

                totalPoints += currentWordPoints;
            }

            return totalPoints;
        }
    }
}
