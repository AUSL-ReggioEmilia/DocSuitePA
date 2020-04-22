using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class DocumentCheckOut_Exception: Exception
    {
        public DocumentCheckOut_Exception()
            : base("Documento estratto da un altro utente")
        {
        }

        public DocumentCheckOut_Exception(string Msg)
            : base(Msg)
        {
        }
    }
}
