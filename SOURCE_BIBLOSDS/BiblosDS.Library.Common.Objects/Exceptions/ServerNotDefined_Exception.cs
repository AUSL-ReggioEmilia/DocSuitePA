using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class ServerNotDefined_Exception: Exception
    {
        public ServerNotDefined_Exception()
            : base("Server non trovato.")
        {
        }

        public ServerNotDefined_Exception(string msg)
            : base(msg)
        {
        }

        public ServerNotDefined_Exception(string format, params object[] args)
            : base(string.Format(format, args)) { }
    }
}
