namespace SuperScrabble.CustomExceptions
{
    using SuperScrabble.ViewModels;

    using System;
    using System.Collections.Generic;

    public abstract class ModelStateFailedException : Exception
    {
        protected ModelStateFailedException(IEnumerable<ModelStateErrorViewModel> errors)
        {
            Errors = errors;
        }

        public IEnumerable<ModelStateErrorViewModel> Errors { get; }
    }
}
