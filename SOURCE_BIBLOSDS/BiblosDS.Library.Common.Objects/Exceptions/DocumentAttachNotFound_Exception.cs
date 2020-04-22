using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class DocumentAttachNotFound_Exception : Exception
    {
        public DocumentAttachNotFound_Exception()
            : base("Nessun allegato trovato con i parametri passati")
        {
        }

        public DocumentAttachNotFound_Exception(string msg)
            : base(msg)
        {
        }
    }
}
