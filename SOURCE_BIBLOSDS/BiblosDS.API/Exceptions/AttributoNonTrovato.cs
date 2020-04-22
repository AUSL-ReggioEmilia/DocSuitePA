using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.API.Exceptions
{
    public class AttributoNonTrovato: Exception
    {
        public AttributoNonTrovato(string msg) : base(msg) { }
    }
}
