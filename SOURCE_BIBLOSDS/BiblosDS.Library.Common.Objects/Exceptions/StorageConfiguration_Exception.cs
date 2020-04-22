using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{

    public class StorageConfiguration_Exception: Exception
    {
        public StorageConfiguration_Exception()
            : base("Storage or Rule not configured correctly.")
        {
        }

        public StorageConfiguration_Exception(string msg)
            : base(msg)
        {
        }
    }
}
