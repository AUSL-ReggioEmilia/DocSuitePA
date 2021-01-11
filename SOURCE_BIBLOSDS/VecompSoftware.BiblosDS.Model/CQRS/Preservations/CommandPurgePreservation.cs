using System;

namespace VecompSoftware.BiblosDS.Model.CQRS.Preservations
{
    public class CommandPurgePreservation : CommandModel
    {
        public CommandPurgePreservation()
            :base()
        {

        }

        public Guid IdPreservation { get; set; }
        public string Executor { get; set; }
    }
}
