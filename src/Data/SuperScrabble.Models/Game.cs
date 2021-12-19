namespace SuperScrabble.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Game
    {
        public Game(string id)
        {
            this.Id = id;
            this.PlayedOn = DateTime.UtcNow;
            this.Users = new HashSet<UserGame>();
        }

        [Key]
        public string Id { get; set; }

        public DateTime PlayedOn { get; set; }

        public virtual ICollection<UserGame> Users { get; set; }
    }
}
