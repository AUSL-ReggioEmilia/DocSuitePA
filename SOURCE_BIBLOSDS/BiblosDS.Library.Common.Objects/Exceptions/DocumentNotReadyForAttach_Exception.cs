using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class DocumentNotReadyForAttach_Exception : Exception
    {
        public DocumentNotReadyForAttach_Exception()
            : base("Documento non ancora caricato: impossibile aggiungere gli allegati.")
        {
        }

        public DocumentNotReadyForAttach_Exception(string Msg)
            : base(Msg)
        {
        }
    }
}
