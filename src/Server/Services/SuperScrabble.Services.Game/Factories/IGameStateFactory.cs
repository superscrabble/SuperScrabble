namespace SuperScrabble.Services.Game.Factories
{
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Common.Enums;
    using SuperScrabble.Services.Game.Models;

    public interface IGameStateFactory
    {
        GameState CreateGameState(
            GameRoomConfiguration roomConfiguration, IEnumerable<Team> teams, string groupName);

        GameState CreateGameState(
            GameMode gameMode, IEnumerable<WaitingTeam> teams, string groupName);
    }
}
