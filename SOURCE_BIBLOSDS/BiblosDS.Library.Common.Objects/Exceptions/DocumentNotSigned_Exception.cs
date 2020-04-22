using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class DocumentNotSigned_Exception : Exception
    {
        public DocumentNotSigned_Exception()
            : base("Documento non firmato.")
        {
        }

        public DocumentNotSigned_Exception(string Msg)
            : base(Msg)
        {
        }
    }  
}
