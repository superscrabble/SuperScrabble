using SuperScrabble.Common.Exceptions.Game;
using SuperScrabble.Services.Data.Users;
using SuperScrabble.Services.Game.Models;
using SuperScrabble.WebApi.ViewModels.Games;

namespace SuperScrabble.Services.Data.Games;

public class GamesService : IGamesService
{
    private readonly IUsersService _usersService;
    private readonly IRepository<PlayedGame> _gamesRepository;

    public GamesService(
        IRepository<PlayedGame> gamesRepository,
        IUsersService usersService)
    {
        _gamesRepository = gamesRepository;
        _usersService = usersService;
    }

    public async Task SaveGameAsync(SaveGameInputModel input)
    {
        var sortedNonLosersByPoints = input.Players
            .Where(p => !p.HasLeftTheGame)
            .OrderByDescending(p => p.Points);

        int maxPoints = sortedNonLosersByPoints.First().Points;

        var gameOutcomesByUserNames = new Dictionary<string, GameOutcome>();

        if (!HasWinner(sortedNonLosersByPoints))
        {
            foreach (Player tiebreaker in GetAllTiebreakers(sortedNonLosersByPoints, maxPoints))
            {
                gameOutcomesByUserNames.Add(tiebreaker.UserName, GameOutcome.Draw);
            }
        }
        else
        {
            gameOutcomesByUserNames.Add(sortedNonLosersByPoints.First().UserName, GameOutcome.Win);
        }

        foreach (Player loser in GetAllLosers(input.Players, maxPoints))
        {
            gameOutcomesByUserNames.Add(loser.UserName, GameOutcome.Loss);
        }

        var game = new PlayedGame(input.GameId);

        foreach (Player player in input.Players)
        {
            AppUser user = await _usersService.GetByUserNameAsync(player.UserName);

            game.Users.Add(new GameUser
            {
                UserId = user.Id,
                GameId = game.Id,
                Points = player.Points,
                GameOutcome = gameOutcomesByUserNames[player.UserName],
                HasLeft = player.HasLeftTheGame,
            });
        }

        await _gamesRepository.AddAsync(game);
        await _gamesRepository.SaveChangesAsync();
    }

    public GameSummaryViewModel GetSummaryById(string id, string userName)
    {
        var game = _gamesRepository.All().FirstOrDefault(x => x.Id == id)
            ?? throw new GameNotFoundException();

        var summary = new GameSummaryViewModel
        {
            PointsByUserNames = game.Users
                .OrderByDescending(x => !x.HasLeft)
                .ThenByDescending(x => x.Points)
                .Select(x => new KeyValuePair<string, int>(x.User.UserName, x.Points))
        };

        var userGame = game.Users.FirstOrDefault(u => u.User.UserName == userName);
        var actualGameOutcome = GameOutcome.None;

        if (userGame != null)
        {
            actualGameOutcome = userGame.GameOutcome;
        }

        var gameOutcomeMessages = new Dictionary<GameOutcome, string>
        {
            [GameOutcome.None] = string.Empty,
            [GameOutcome.Loss] = "Loss",
            [GameOutcome.Win] = "Win",
            [GameOutcome.Draw] = "Draw",
        };

        summary.GameOutcomeMessage = gameOutcomeMessages[actualGameOutcome];
        summary.GameOutcomeNumber = (int)actualGameOutcome;

        return summary;
    }
    private static bool HasWinner(IOrderedEnumerable<Player> sortedPlayersByPoints)
    {
        Player first = sortedPlayersByPoints.First();
        Player? second = sortedPlayersByPoints.Skip(1).FirstOrDefault();

        return first.Points != second?.Points;
    }

    private static IEnumerable<Player> GetAllTiebreakers(
        IOrderedEnumerable<Player> sortedPlayersByPoints, int maxPoints)
    {
        return sortedPlayersByPoints.Where(p => p.Points == maxPoints);
    }

    private static IEnumerable<Player> GetAllLosers(IEnumerable<Player> sortedPlayersByPoints, int maxPoints)
    {
        return sortedPlayersByPoints.Where(p => p.HasLeftTheGame || p.Points < maxPoints);
    }
}
