namespace SuperScrabble.Data.Models
{
    public class Word
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(WordValueMaxLength)]
        public string Value { get; set; } = default!;
    }
}
