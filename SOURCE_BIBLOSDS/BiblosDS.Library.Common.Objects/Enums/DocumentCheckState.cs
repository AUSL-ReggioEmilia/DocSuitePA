using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Enums
{
    public enum DocumentCheckState
    {
        FileModified = -2,
        MetaDataModified = -1,
        Ok = 1,
        
    }
}
