using System;

namespace VecompSoftware.BiblosDS.Model.CQRS
{
    public abstract class CommandModel
    {
        public CommandModel()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string ReferenceId { get; set; }
    }
}
