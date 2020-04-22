using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Objects
{
    public class DocumentArchiveCertificate
    {
        public string FileName { get; set; }
        public string UserName { get; set; }
        public string Pin { get; set; }
        public Byte[] CertificateBlob { get; set; }

        public Guid IdArchiveCertificate { get; set; }
    }
}
