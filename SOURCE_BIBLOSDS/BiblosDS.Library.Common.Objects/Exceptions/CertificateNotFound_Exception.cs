using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class CertificateNotFound_Exception: Exception
    {
        public CertificateNotFound_Exception()
            : base("Certificato di default non trovato.")
        {
        }

        public CertificateNotFound_Exception(string msg)
            : base(msg)
        {
        }
    }
}
