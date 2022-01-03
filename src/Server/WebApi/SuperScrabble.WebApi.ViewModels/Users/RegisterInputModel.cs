namespace SuperScrabble.WebApi.ViewModels.Users
{
    using SuperScrabble.Common.Attributes;

    using static SuperScrabble.WebApi.Resources.UserValidationErrorCodes;

    public class RegisterInputModel
    {
        [Required(ErrorMessage = UserNameRequired)]
        public string UserName { get; set; } = default!;

        [ValidEmailAddress]
        [Required(ErrorMessage = EmailRequired)]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = PasswordRequired)]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = RepeatedPasswordRequired)]
        [Compare(nameof(Password), ErrorMessage = PasswordsDoNotMatch)]
        public string RepeatedPassword { get; set; } = default!;
    }
}
