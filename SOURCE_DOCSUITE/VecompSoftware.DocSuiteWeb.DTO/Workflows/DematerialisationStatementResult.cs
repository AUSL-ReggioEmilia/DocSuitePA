using System;
using System.ComponentModel;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.DTO.Workflows
{
    public class DematerialisationStatementResult
    {
        /// <summary>
        /// Costruttore vuoto
        /// </summary>
        public DematerialisationStatementResult() : base()
        { }

        public Guid UDId { get; set; }
        public short Year { get; set; }
        public int Number { get; set; }

        public string Title { get; set; }
        public string UDName { get; set; }

        public string ContainerName { get; set; }

        public string Subject { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }
    }
}
