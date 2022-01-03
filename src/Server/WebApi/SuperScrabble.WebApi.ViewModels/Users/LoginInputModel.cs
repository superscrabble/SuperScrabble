namespace SuperScrabble.WebApi.ViewModels.Users
{
    using static SuperScrabble.WebApi.Resources.UserValidationErrorCodes;

    public class LoginInputModel
    {
        [Required(ErrorMessage = UserNameRequired)]
        public string UserName { get; set; } = default!;

        [Required(ErrorMessage = PasswordRequired)]
        public string Password { get; set; } = default!;
    }
}
