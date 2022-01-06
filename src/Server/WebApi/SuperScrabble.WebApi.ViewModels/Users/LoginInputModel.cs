namespace SuperScrabble.WebApi.ViewModels.Users
{
    using static SuperScrabble.WebApi.Resources.User;
    using static SuperScrabble.WebApi.Resources.User.ErrorCodes;

    public class LoginInputModel
    {
        [Display(Name = PropertyNames.Email)]
        [Required(ErrorMessage = UserNameRequired)]
        public string UserName { get; set; } = default!;

        [Display(Name = PropertyNames.Password)]
        [Required(ErrorMessage = PasswordRequired)]
        public string Password { get; set; } = default!;
    }
}
