using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Protocols
{
    public class ProtocolParerModel
    {
        public DateTime? ArchiviedDate { get; set; }

        public bool? HasError { get; set; }

        public short? IsForArchive { get; set; }

        public string LastError { get; set; }

        public DateTime? LastSendDate { get; set; }

        public int? Number { get; set; }

        public string ParerUri { get; set; }

        public short? Year { get; set; }
    }
}
