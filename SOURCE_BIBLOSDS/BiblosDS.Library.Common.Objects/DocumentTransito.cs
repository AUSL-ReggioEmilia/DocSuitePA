using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Enums;

namespace BiblosDS.Library.Common.Objects
{
    public partial class DocumentTransito: BiblosDSObject
    {
        public Guid IdTransit { get; set; }
        public Guid? IdServer { get; set; }
        public Guid IdDocument { get; set; }
        public short Status { get; set; }
        public string LocalPath { get; set; }        
        public int Retry { get; set; }
        public DateTime DateRetry { get; set; }
        public Document Document { get; set; }
        public string ServerName { get; set; }

        public DocumentTarnsitoStatus TarnsitoStatus
        {
            get
            {
                return (DocumentTarnsitoStatus)Status;
            }
            set
            {
                Status = (short)value;
            }
        }
    }
}
