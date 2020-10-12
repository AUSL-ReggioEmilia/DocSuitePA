using System;

namespace VecompSoftware.BiblosDS.Model.CQRS.Preservations
{
    public class CommandExecutePreservation : CommandModel
    {
        public Guid IdTask { get; set; }
        public bool AutoGenerateNextTask { get; set; }
        public string PDVArchive { get; set; }
        public string RDVArchive { get; set; }
    }
}
