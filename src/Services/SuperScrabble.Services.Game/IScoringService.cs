namespace SuperScrabble.Services.Game
{
    using System.Collections.Generic;

    using SuperScrabble.InputModels.Game;
    using SuperScrabble.Services.Game.Models;

    public interface IScoringService
    {
        int CalculatePointsFromPlayerInput(WriteWordInputModel input, IBoard board, IEnumerable<WordBuilder> words);
    }
}
