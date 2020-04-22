using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class FileNotUploaded_Exception: Exception
    {
        public FileNotUploaded_Exception()
            : base("Inpossibile caricare il documento")
        {
        }

        public FileNotUploaded_Exception(string msg): base(msg)
        {
        }

    }
}
