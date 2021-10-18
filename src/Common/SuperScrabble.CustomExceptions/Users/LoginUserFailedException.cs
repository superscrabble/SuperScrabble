namespace SuperScrabble.CustomExceptions.Users
{
    using SuperScrabble.ViewModels;
    using System.Collections.Generic;

    public class LoginUserFailedException : UserOperationFailedException
    {
        public LoginUserFailedException(IEnumerable<ModelStateErrorViewModel> errors) : base(errors)
        {
        }
    }
}
