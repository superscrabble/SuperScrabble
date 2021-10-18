namespace SuperScrabble.CustomExceptions.Users
{
    using SuperScrabble.ViewModels;
    using System.Collections.Generic;

    public class DeleteUserFailedException : UserOperationFailedException
    {
        public DeleteUserFailedException(IEnumerable<ModelStateErrorViewModel> errors) : base(errors)
        {
        }
    }
}
