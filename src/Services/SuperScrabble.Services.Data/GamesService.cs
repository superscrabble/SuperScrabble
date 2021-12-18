namespace SuperScrabble.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using SuperScrabble.Models;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.Data.Repositories;
    using SuperScrabble.Services.Game.Models;

    public class GamesService : IGamesService
    {
        private readonly IUsersService usersService;
        private readonly IRepository<Game> gamesRepository;

        public GamesService(IUsersService usersService, IRepository<Game> gamesRepository)
        {
            this.usersService = usersService;
            this.gamesRepository = gamesRepository;
        }

        public async Task SaveGameAsync(SaveGameInputModel input)
        {
            var sortedPlayersByPoints = input.Players.OrderByDescending(p => p.Points);

            int maxPoints = sortedPlayersByPoints.First().Points;

            var gameOutcomesByUserNames = new Dictionary<string, GameOutcome>();

            if (!HasWinner(sortedPlayersByPoints))
            {
                foreach(Player tiebreaker in GetAllTiebreakers(sortedPlayersByPoints, maxPoints))
                {
                    gameOutcomesByUserNames.Add(tiebreaker.UserName, GameOutcome.Draw);
                }
            }
            else
            {
                gameOutcomesByUserNames.Add(sortedPlayersByPoints.First().UserName, GameOutcome.Win);
            }

            foreach (Player loser in GetAllLosers(sortedPlayersByPoints, maxPoints))
            {
                gameOutcomesByUserNames.Add(loser.UserName, GameOutcome.Loss);
            }

            var game = new Game();

            foreach (Player player in sortedPlayersByPoints)
            {
                AppUser user = await this.usersService.GetAsync(player.UserName);

                game.Users.Add(new UserGame
                {
                    UserId = user.Id,
                    GameId = game.Id,
                    Points = player.Points,
                    GameOutcome = gameOutcomesByUserNames[player.UserName],
                });
            }

            await this.gamesRepository.AddAsync(game);
            await this.gamesRepository.SaveChangesAsync();
        }

        private static bool HasWinner(IOrderedEnumerable<Player> sortedPlayersByPoints)
        {
            Player first = sortedPlayersByPoints.First();
            Player second = sortedPlayersByPoints.Skip(1).First();

            return first.Points != second.Points;
        }

        private static IEnumerable<Player> GetAllTiebreakers(IOrderedEnumerable<Player> sortedPlayersByPoints, int maxPoints)
        {
            return sortedPlayersByPoints.Where(p => p.Points == maxPoints);
        }

        private static IEnumerable<Player> GetAllLosers(IOrderedEnumerable<Player> sortedPlayersByPoints, int maxPoints)
        {
            return sortedPlayersByPoints.Where(p => p.Points < maxPoints);
        }
    }
}
