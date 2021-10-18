namespace SuperScrabble.InputModels
{
    using SuperScrabble.CustomValidationAttributes.Password;
    using System.ComponentModel.DataAnnotations;

    public class UpdateInputModel
    {
        [Required]
        public string UserName { get; init; }

        [Required]
        public string NewUserName { get; init; }
        
        [Required]
        [PasswordMinLength]
        public string Password { get; init; }

        [Required]
        [PasswordMinLength]
        public string NewPassword { get; init; }

        [Required]
        public string Email { get; init; }

        [Required]
        public string NewEmail { get; init; }
    }
}
