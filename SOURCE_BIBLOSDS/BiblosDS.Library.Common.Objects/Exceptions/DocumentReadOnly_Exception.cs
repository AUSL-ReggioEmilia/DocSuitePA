using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class DocumentReadOnly_Exception: Exception
    {
        public DocumentReadOnly_Exception()
            : base("Documento non modificabile.")
        {
        }

        public DocumentReadOnly_Exception(string msg)
            : base(msg)
        {
        }
    }
}
