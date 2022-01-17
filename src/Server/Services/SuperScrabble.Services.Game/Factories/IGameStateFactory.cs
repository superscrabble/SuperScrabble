namespace SuperScrabble.Services.Game.Factories
{
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Models;

    public interface IGameStateFactory
    {
        public GameState CreateGameState(
            GameRoomConfiguration roomConfiguration, IEnumerable<Team> teams, string groupName);
    }
}
