using System;

namespace VecompSoftware.BiblosDS.Model.CQRS.Preservations
{
    public class CommandInsertPreservationPDV : CommandModel
    {
        public string PDVArchive { get; set; }
        public Guid IdAwardBatch { get; set; }
        public string Content { get; set; }
    }
}
