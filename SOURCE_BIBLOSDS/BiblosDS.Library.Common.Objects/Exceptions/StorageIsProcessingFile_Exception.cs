using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class StorageIsProcessingFile_Exception: Exception
    {
        public StorageIsProcessingFile_Exception()
            : base("Lo storage sta già processando il file selezioanto!")
        {
        }

        public StorageIsProcessingFile_Exception(string msg)
            : base(msg)
        {
        }
    }
}
