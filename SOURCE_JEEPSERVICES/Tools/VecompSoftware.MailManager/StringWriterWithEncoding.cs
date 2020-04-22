using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VecompSoftware.MailManager
{

    public class StringWriterWithEncoding : StringWriter
    {
        private Encoding myEncoding;
        public override Encoding Encoding
        {
            get
            {
                return myEncoding;
            }
        }
        public StringWriterWithEncoding(Encoding encoding)
            : base()
        {
            myEncoding = encoding;
        }
    }

}
