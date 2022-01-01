namespace SuperScrabble.Data.Models
{
    public class Game
    {
        public Game(string id)
        {
            this.Id = id;
            this.PlayedOn = DateTime.UtcNow;
            this.Users = new HashSet<GameUser>();
        }

        [Key]
        public string Id { get; set; }

        public DateTime PlayedOn { get; set; }

        public virtual ICollection<GameUser> Users { get; set; }
    }
}
