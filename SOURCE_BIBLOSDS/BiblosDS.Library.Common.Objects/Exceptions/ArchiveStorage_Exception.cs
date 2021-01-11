using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class Archive_Exception : Exception
    {
        public Archive_Exception()
            : base("Nessun Archive trovato con i parametri passati")
        {
        }

        public Archive_Exception(string msg)
            : base(msg)
        {
        }
    }

    public class ArchiveStorage_Exception: Exception
    {
        public ArchiveStorage_Exception()
            : base("Nessun Archive Storage trovato con i parametri passati")
        {
        }

        public ArchiveStorage_Exception(string msg)
            : base(msg)
        {
        }
    }
}
