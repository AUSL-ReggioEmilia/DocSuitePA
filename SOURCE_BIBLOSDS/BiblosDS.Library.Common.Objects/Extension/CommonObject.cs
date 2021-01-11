using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Objects;

namespace BiblosDS
{
    public static class CommonObject
    {
        public static bool IsSigned(this Document doc)
        {
            if (doc == null)
                return false;
            return doc.Name.IsSignedFile();                       
        }
    }
}
