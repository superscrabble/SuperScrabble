namespace SuperScrabble.Services.Game.Matchmaking
{
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Models;

    public interface IMatchmakingService
    {
        void AddTeamToWaitingQueue(
            GameRoomConfiguration roomConfiguration, Team teamToAdd, out bool hasGameStarted);

        void AddPlayerToLobby(
            GameRoomConfiguration roomConfiguration, Player player, out bool isLobbyReady);

        bool IsUserAlreadyWaitingToJoinGame(string userName);

        bool IsUserAlreadyInsideGame(string userName);

        GameState? GetGameState(string userName);
    }
}
