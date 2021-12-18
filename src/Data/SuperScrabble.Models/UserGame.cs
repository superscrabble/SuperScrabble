namespace SuperScrabble.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class UserGame
    {
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public virtual AppUser User { get; set; }

        [ForeignKey(nameof(Game))]
        public string GameId { get; set; }

        public virtual Game Game { get; set; }

        public int Points { get; set; }

        public GameOutcome GameOutcome { get; set; }
    }
}
