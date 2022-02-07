namespace SuperScrabble.Services.Game.Matchmaking
{
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Models;

    public interface IMatchmakingService
    {
        string CreateFriendlyGame(string creatorUserName,
            string creatorConnectionId, FriendlyGameConfiguration gameConfig);

        void JoinFriendlyGame(string joinerUserName, string joinerConnectionId, string invitationCode);

        void StartFriendlyGame(string starterUserName, string invitationCode);

        FriendlyGameLobby GetFriendlyGameLobby(string invitationCode);

        GameState GetGameState(string userName);

        void AddTeamToWaitingQueue(
            GameRoomConfiguration roomConfiguration, Team teamToAdd, out bool hasGameStarted);

        void AddPlayerToLobby(
            GameRoomConfiguration roomConfiguration, Player player, out bool isLobbyReady);

        bool IsUserAlreadyWaitingToJoinGame(string userName);

        bool IsUserAlreadyInsideGame(string userName);
    }
}
