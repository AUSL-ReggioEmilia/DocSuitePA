using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Audits
{
    public class AuditModel
    {
        public AuditModel()
        {
            AuditId = Guid.NewGuid();
        }
        public Guid AuditId { get; set; }
        public Guid EntityUniqueId { get; set; }
        public string EntityName { get; set; }
        public DateTimeOffset LogDate { get; set; }
        public string UserHost { get; set; }
        public string Account { get; set; }
        public short Type { get; set; }
        public string Description { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
    }
}
