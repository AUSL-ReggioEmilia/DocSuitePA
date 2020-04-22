using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Conservations
{
    public class ConservationLogModel
    {
        public Guid Id { get; set; }

        public string EntityName { get; set; }

        public DateTimeOffset? LogDate { get; set; }

        public string RegistrationUser { get; set; }

        public string Description { get; set; }

        public string Hash { get; set; }

        public string ReferenceEntityName { get; set; }

        public Guid? ReferenceUniqueId { get; set; }

        public string Subject { get; set; }

        public short? Year { get; set; }

        public int? Number { get; set; }

        public string LogType { get; set; }
    }
}
