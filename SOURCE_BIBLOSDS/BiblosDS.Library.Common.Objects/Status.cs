using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Objects
{
    public partial class Status : BiblosDSObject
    {
        public short IdStatus { get; set; }
        public string Description { get; set; }

        public Status()
        {
        }

        public Status(BiblosDS.Library.Common.Enums.DocumentStatus Status)
        {
            this.IdStatus = (short)Status;
        }
        
        public Status(short IdStatus)
        {
            this.IdStatus = IdStatus;
        }
    }
}
