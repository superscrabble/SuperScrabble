namespace SuperScrabble.Services.Game
{
    using SuperScrabble.Services.Game.Models;
    using System.Collections.Generic;

    public interface IGameStateProvider
    {
        GameState GetGameState(string userName);

        GameState GetGameStateByGroupName(string groupName);

        bool IsUserAlreadyWaiting(string userName, string connectionId);

        bool IsUserAlreadyInsideGame(string userName);

        void AddUserToWaitingList(string userName, string connectionId);

        bool IsUserInsideGroup(string userName, string groupName);

        string CreateGroupFromWaitingPlayers();

        int WaitingPlayersCount { get; }

        bool IsWaitingQueueFull { get; }

        void ClearWaitingQueue();

        int NeededPlayersCount { get; }

        void AddGameStateToGroup(GameState gameState, string groupName);

        void RemoveUserFromWaitingQueue(string userName);

        IEnumerable<KeyValuePair<string, string>> GetWaitingPlayers(string groupName);
    }
}
