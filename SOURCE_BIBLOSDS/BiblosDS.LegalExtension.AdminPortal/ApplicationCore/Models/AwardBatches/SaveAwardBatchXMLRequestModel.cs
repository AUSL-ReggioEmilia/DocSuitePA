using System;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.AwardBatches
{
    public class SaveAwardBatchXMLRequestModel
    {
        public string ArchiveName { get; set; }
        public Guid IdAwardBatch { get; set; }
        public byte[] Content { get; set; }
    }
}