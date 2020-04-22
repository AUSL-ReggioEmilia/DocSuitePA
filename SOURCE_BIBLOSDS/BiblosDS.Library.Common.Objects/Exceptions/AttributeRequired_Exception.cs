using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class AttributeRequired_Exception: Exception
    {
        public AttributeRequired_Exception(string AttributeName)
            : base(AttributeName)
        {
        }
    }
}
