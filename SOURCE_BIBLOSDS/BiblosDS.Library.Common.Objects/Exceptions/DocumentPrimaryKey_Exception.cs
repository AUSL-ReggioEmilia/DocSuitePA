using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class DocumentPrimaryKey_Exception: Exception
    {
        public DocumentPrimaryKey_Exception()
            : base("Violazione del vincolo di chiave univoca")
        {
        }

        public DocumentPrimaryKey_Exception(string Msg)
            : base(Msg)
        {
        }
    }
}
