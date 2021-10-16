namespace SuperScrabble.CustomExceptions
{
    using SuperScrabble.ViewModels;

    using System.Collections.Generic;

    public class RegisterFailedException : ModelStateFailedException
    {
        public RegisterFailedException(IEnumerable<ModelStateErrorViewModel> errors) : base(errors)
        {
        }
    }
}
