namespace SuperScrabble.CustomExceptions.Users
{
    using SuperScrabble.ViewModels;
    using System.Collections.Generic;

    public class GetUserFailedException : UserOperationFailedException
    {
        public GetUserFailedException(IEnumerable<ModelStateErrorViewModel> errors) : base(errors)
        {
        }
    }
}
