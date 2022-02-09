namespace SuperScrabble.Services.Game.Matchmaking
{
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.WebApi.ViewModels.Game;

    public interface IMatchmakingService
    {
        string CreateFriendlyGame(
            string creatorUserName, string creatorConnectionId, CreateFriendlyGameInputModel input);

        void JoinFriendlyGame(string joinerName, string joinerConnectionId, string invitationCode, out bool canGameBeStarted);

        void AddTeamToWaitingQueue(
            GameRoomConfiguration roomConfiguration, Team teamToAdd, out bool hasGameStarted);

        void AddPlayerToLobby(
            GameRoomConfiguration roomConfiguration, Player player, out bool isLobbyReady);

        bool IsUserAlreadyWaitingToJoinGame(string userName);

        bool IsUserAlreadyInsideGame(string userName);

        GameState? GetGameState(string userName);
    }
}
