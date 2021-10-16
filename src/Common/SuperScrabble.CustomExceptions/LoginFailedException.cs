namespace SuperScrabble.CustomExceptions
{
    using SuperScrabble.ViewModels;

    using System.Collections.Generic;

    public class LoginFailedException : ModelStateFailedException
    {
        public LoginFailedException(IEnumerable<ModelStateErrorViewModel> errors) : base(errors)
        {
        }
    }
}
