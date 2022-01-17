namespace SuperScrabble.Services.Game.Matchmaking
{
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Models;

    public interface IMatchmakingService
    {
        void AddTeamToWaitingQueue(
            GameRoomConfiguration roomConfiguration, Team teamToAdd, out bool hasGameStarted);
    }
}
