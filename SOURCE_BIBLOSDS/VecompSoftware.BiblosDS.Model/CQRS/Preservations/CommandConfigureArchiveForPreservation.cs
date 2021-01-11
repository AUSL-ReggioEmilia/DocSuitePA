using System;

namespace VecompSoftware.BiblosDS.Model.CQRS.Preservations
{
    public class CommandConfigureArchiveForPreservation : CommandModel
    {
        public CommandConfigureArchiveForPreservation()
        {

        }

        public Guid IdArchive { get; set; }
        public Guid IdCompany { get; set; }
    }
}
