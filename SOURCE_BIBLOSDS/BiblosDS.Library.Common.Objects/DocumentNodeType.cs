using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Objects
{
    public partial class DocumentNodeType : BiblosDSObject
    {
        public short IdNodeType { get; set; }
        public string Description { get; set; }

        public DocumentNodeType()
        {
        }

        public DocumentNodeType(short IdNodeType)
        {
            this.IdNodeType = IdNodeType;
        }
    }
}
