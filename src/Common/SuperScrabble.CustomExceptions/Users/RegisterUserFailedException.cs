namespace SuperScrabble.CustomExceptions.Users
{
    using SuperScrabble.ViewModels;
    using System.Collections.Generic;

    public class RegisterUserFailedException : UserOperationFailedException
    {
        public RegisterUserFailedException(IEnumerable<ModelStateErrorViewModel> errors) : base(errors)
        {
        }
    }
}
