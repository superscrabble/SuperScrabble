namespace SuperScrabble.Services.Game.GameStateManagers
{
    using System.Collections.Generic;

    using SuperScrabble.Services.Game.Models;

    public interface IGameStateManager
    {
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

        void RemoveUserFromGroup(string userName);

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

        void RemoveGameState(string groupName);

        IEnumerable<KeyValuePair<string, string>> GetWaitingPlayers();
    }
}
