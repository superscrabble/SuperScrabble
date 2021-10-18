namespace SuperScrabble.CustomExceptions.Users
{
    using SuperScrabble.ViewModels;
    using System.Collections.Generic;

    public class UpdateUserFailedException : UserOperationFailedException
    {
        public UpdateUserFailedException(IEnumerable<ModelStateErrorViewModel> errors) : base(errors)
        {
        }
    }
}
