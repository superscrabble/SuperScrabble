namespace SuperScrabble.Services.Game.GameStateManagers
{
    using System.Collections.Generic;

    using SuperScrabble.Services.Game.Models;

    public interface IGameStateManager
    {
        int WaitingPlayersCount { get; }

        bool IsWaitingQueueFull { get; }

        int NeededPlayersCount { get; }

        /// <summary>
        /// Gets the game state of the given user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>The game state of the given user or null if the username is not valid</returns>
        GameState GetGameState(string userName);

        /// <summary>
        /// Gets the game state of the given group
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns>The game state of the given group or null if the group name is not valid</returns>
        GameState GetGameStateByGroupName(string groupName);

        /// <summary>
        /// Gets the group name of the given user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>The group name of the given user or null if the username is not valid</returns>
        string GetGroupName(string userName);

        string CreateGroupFromWaitingPlayers();

        void RemoveUserFromGroup(string userName);

        void ClearWaitingQueue();

        void AddUserToWaitingList(string userName, string connectionId);

        void AddGameStateToGroup(GameState gameState, string groupName);

        void RemoveUserFromWaitingQueue(string userName);

        void RemoveGameState(string groupName);

        bool IsUserInsideGroup(string userName, string groupName);

        bool IsUserAlreadyWaiting(string userName, string connectionId);

        bool IsUserAlreadyInsideGame(string userName);

        IEnumerable<KeyValuePair<string, string>> GetWaitingPlayers();
    }
}
