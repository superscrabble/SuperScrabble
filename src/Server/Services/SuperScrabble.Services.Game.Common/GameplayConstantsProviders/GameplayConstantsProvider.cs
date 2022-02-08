﻿namespace SuperScrabble.Services.Game.Common.GameplayConstantsProviders
{
    public class GameplayConstantsProvider : IGameplayConstantsProvider
    {
        public int PlayerTilesCount => 7;

        public int PlayersPerGameCount => 2;

        public int MinSkipsCountForEachPlayerToEndTheGame => 2;

        public int BonusPointsForUsingAllTiles => 50;

        public int GameTimerSeconds => 65;

        public int MinWordLettersCount => 2;
    }
}