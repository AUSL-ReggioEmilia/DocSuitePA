using System;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public class UDSLogModel
    {
        public Guid UDSLogId { get; set; }
        public Guid UDSId { get; set; }
        public DateTimeOffset LogDate { get; set; }
        public string SystemComputer { get; set; }
        public string SystemUser { get; set; }
        public UDSLogType LogType { get; set; }
        public string LogDescription { get; set; }
        public short? Severity { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string LastChangedUser { get; set; }
    }
}
