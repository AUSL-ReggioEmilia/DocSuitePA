using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class StorageAreaConfiguration_Exception: Exception
    {
        public StorageAreaConfiguration_Exception()
            : base("Storage Area not configurata correttamente.")
        {
        }

        public StorageAreaConfiguration_Exception(string msg)
            : base(msg)
        {
        }
    }
}
