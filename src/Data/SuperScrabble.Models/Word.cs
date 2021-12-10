namespace SuperScrabble.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Word
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
