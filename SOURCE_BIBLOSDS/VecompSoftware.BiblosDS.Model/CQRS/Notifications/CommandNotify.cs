using System.Collections.Generic;

namespace VecompSoftware.BiblosDS.Model.CQRS.Notifications
{
    public class CommandNotify : CommandModel
    {
        public CommandNotify()
        {
            Details = new List<NotificationDetailModel>();
        }
        public NotifyLevel NotifyLevel { get; set; }
        public string Message { get; set; }
        public bool Complete { get; set; }
        public ICollection<NotificationDetailModel> Details { get; set; }
    }
}
