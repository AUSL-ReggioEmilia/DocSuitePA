using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    /// <summary>
    /// No storage found.
    /// </summary>
    public class StorageNotFound_Exception: Exception
    {
        public StorageNotFound_Exception()
            : base("Nessuno storage configurato per l'archivio selezionato!")
        {
        }

        public StorageNotFound_Exception(string msg)
            : base(msg)
        {
        }
    }
}
