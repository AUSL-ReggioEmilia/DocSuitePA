using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Enums;

namespace BiblosDS.Library.Common.Exceptions
{
    public class DocumentWithoutContent_Exception : Exception
    {
        public DocumentWithoutContent_Exception()
            : base("Documento senza file.")
        {
        }

        public DocumentWithoutContent_Exception(DocumentStatus status)
            : base("Documento in stato: "+ status + ". Nessun file presente.")
        {
        }
    }
}
