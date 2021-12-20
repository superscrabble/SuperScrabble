namespace SuperScrabble.Services.Game.Scoring
{
    using System.Collections.Generic;

    using SuperScrabble.InputModels.Game;
    using SuperScrabble.Services.Game.Models;

    public interface IScoringService
    {
        int CalculatePointsFromPlayerInput(WriteWordInputModel input, IBoard board, IEnumerable<WordBuilder> words);
    }
}
