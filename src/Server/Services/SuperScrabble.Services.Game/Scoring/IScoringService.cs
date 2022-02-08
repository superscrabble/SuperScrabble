﻿namespace SuperScrabble.Services.Game.Scoring
{
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.Models.Boards;

    using SuperScrabble.WebApi.ViewModels.Game;

    public interface IScoringService
    {
        int CalculatePointsFromPlayerInput(
            WriteWordInputModel input, IBoard board, IEnumerable<WordBuilder> words);
    }
}