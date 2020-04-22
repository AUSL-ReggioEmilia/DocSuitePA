using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.API.Exceptions
{
    public class ErroreChiamataAlServizioRemoto: Exception
    {
        public ErroreChiamataAlServizioRemoto(string msg) : base(msg) { }
    }
}
