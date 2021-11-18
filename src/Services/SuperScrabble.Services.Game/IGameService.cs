namespace SuperScrabble.Services.Game
{
    using System.Collections.Generic;

    public interface IGameService
    {
        GameState CreateGame(IEnumerable<string> userNames);
    }
}
