namespace SuperScrabble.Services.Game.Common
{
    using System.ComponentModel.DataAnnotations;

    using SuperScrabble.Services.Game.Common.Enums;

    public struct GameRoomConfiguration
    {
        public const int MinTeamsCount = 2;
        public const int MaxTeamsCount = 4;

        [Range(MinTeamsCount, MaxTeamsCount)]
        public int TeamsCount { get; set; }

        public TimerDifficulty TimerDifficulty { get; set; }

        public TeamType TeamType { get; set; }

        public TimerType TimerType { get; set; }

        public PartnerType? PartnerType { get; set; }

        //GameMode = Classic, MadBoards, 
    }
}
