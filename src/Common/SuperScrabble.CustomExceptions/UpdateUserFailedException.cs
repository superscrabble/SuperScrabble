using SuperScrabble.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.CustomExceptions
{
    public class UpdateUserFailedException : ModelStateFailedException
    {
        public UpdateUserFailedException(IEnumerable<ModelStateErrorViewModel> errors) : base(errors)
        {
        }
    }
}
