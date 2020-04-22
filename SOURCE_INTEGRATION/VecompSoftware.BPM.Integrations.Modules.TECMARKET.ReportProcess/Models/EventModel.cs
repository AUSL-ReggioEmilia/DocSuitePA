using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.ReportProcess.Models
{
    public class EventModel
    {
        public string EventDate { get; set; }
        public string ServerHost { get; set; }
        public string ServerIP { get; set; }
        public List<EventLogModel> EventLogs { get; set; }
    }
}
