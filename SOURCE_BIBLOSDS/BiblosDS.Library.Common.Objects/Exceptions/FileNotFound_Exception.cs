using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class FileNotFound_Exception: Exception
    {
        public FileNotFound_Exception()
            : base("File non trovato")
        {
        }

        public FileNotFound_Exception(string msg):base(msg)
        {
        }
    }
}
