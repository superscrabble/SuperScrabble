namespace SuperScrabble.WebApi.ViewModels.Users
{
    using SuperScrabble.Common.Attributes;

    using static SuperScrabble.WebApi.Resources.User;
    using static SuperScrabble.WebApi.Resources.User.ErrorCodes;

    public class RegisterInputModel
    {
        [Display(Name = PropertyNames.Email)]
        [Required(ErrorMessage = UserNameRequired)]
        public string UserName { get; set; } = default!;

        [ValidEmailAddress]
        [Display(Name = PropertyNames.UserName)]
        [Required(ErrorMessage = EmailRequired)]
        public string Email { get; set; } = default!;

        [Display(Name = PropertyNames.Password)]
        [Required(ErrorMessage = PasswordRequired)]
        public string Password { get; set; } = default!;

        [Display(Name = PropertyNames.RepeatedPassword)]
        [Required(ErrorMessage = RepeatedPasswordRequired)]
        [Compare(nameof(Password), ErrorMessage = PasswordsDoNotMatch)]
        public string RepeatedPassword { get; set; } = default!;
    }
}
