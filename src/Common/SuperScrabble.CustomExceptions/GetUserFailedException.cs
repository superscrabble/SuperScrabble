using SuperScrabble.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.CustomExceptions
{
    public class GetUserFailedException : ModelStateFailedException
    {
        public GetUserFailedException(IEnumerable<ModelStateErrorViewModel> errors) : base(errors)
        {
        }
    }
}
