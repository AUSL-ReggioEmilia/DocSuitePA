using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class DocumentNotFound_Exception: Exception
    {
        public DocumentNotFound_Exception()
            : base("Nessun documento trovato con i parametri passati")
        {
        }

        public DocumentNotFound_Exception(string msg)
            : base(msg)
        {
        }

        public DocumentNotFound_Exception(string format, params object[] args)
            : base(string.Format(format, args)) { }

    }
}
