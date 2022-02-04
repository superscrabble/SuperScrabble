namespace SuperScrabble.Data.Models
{
    public class GameUser
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Game))]
        public string GameId { get; set; } = default!;

        public virtual Game Game { get; set; } = default!;

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = default!;

        public virtual AppUser User { get; set; } = default!;

        public int Points { get; set; }

        public GameOutcome GameOutcome { get; set; }

        public bool HasLeft { get; set; }
    }
}
