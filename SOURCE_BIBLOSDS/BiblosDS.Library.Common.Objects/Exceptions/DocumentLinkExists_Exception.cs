using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class DocumentConnectionExists_Exception : Exception
    {
        public DocumentConnectionExists_Exception()
            : base("Il collegamento fra documenti risulta già presente.")
        {
        }

        public DocumentConnectionExists_Exception(string msg)
            : base(msg)
        {
        }
    }
}
