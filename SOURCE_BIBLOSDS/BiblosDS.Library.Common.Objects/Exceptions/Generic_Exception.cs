using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class Generic_Exception: Exception
    {
        public Generic_Exception(string msg)
            : base(msg)
        {
        }

        public Generic_Exception(string msg, params object[] args)
            : base(string.Format(msg, args))
        {
        }
    }
}
