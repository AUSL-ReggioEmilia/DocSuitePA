using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class Attribute_Exception: Exception
    {
        public Attribute_Exception(string msg)
            : base(msg)
        {
        }
    }
}
