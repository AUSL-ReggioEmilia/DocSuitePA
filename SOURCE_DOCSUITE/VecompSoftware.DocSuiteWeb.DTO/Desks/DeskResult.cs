using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.DTO.Desks
{
    public class DeskResult
    {
        /// <summary>
        /// Costruttore vuoto
        /// </summary>
        public DeskResult() : base()
        { }

        public Guid DeskId { get; set; }

        public string DeskName { get; set; }

        public string ContainerName { get; set; }

        public DeskState? DeskState { get; set; }

        public string DeskSubject { get; set; }

        public DateTime? DeskExpirationDate { get; set; }
    }
}
