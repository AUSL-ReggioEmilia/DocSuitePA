using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class DocumentDateNotValid_Exception : Exception
    {
        public DocumentDateNotValid_Exception()
            : base("La data principale del documento è antecedente l'ultimo documento conservato.")
        {
        }

        public DocumentDateNotValid_Exception(string msg)
            : base(msg)
        {
        }
    }
}
