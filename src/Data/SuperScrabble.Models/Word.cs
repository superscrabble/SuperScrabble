namespace SuperScrabble.Models
{
    using System.ComponentModel.DataAnnotations;

    using static SuperScrabble.Data.Common.EntityConstraints;

    public class Word
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(WordValueMaxLength)]
        public string Value { get; set; }
    }
}
