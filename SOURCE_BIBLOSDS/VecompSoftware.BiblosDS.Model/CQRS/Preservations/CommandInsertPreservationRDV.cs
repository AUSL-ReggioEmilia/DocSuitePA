using System;

namespace VecompSoftware.BiblosDS.Model.CQRS.Preservations
{
    public class CommandInsertPreservationRDV : CommandModel
    {
        public string RDVArchive { get; set; }
        public Guid IdAwardBatch { get; set; }
        public string Content { get; set; }
    }
}
