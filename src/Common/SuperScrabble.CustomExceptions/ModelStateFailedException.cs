namespace SuperScrabble.CustomExceptions
{
    using SuperScrabble.ViewModels;

    using System;
    using System.Collections.Generic;

    public abstract class ModelStateFailedException : Exception
    {
        public IEnumerable<ModelStateErrorViewModel> Errors { get; init; }
    }
}
