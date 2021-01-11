using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class DocumentNotConvertible_Exception: Exception
    {
        public DocumentNotConvertible_Exception()
            : base("Documento in un formato non convertibile.")
        {
        }

        public DocumentNotConvertible_Exception(string msg)
            : base(msg)
        {
        }    
    }
}
