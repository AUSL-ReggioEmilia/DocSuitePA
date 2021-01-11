using System;

namespace VecompSoftware.BiblosDS.Model.CQRS.Notifications
{
    public class NotificationDetailModel
    {
        public Guid Id { get; set; }
        public string ActivityName { get; set; }
        public string Detail { get; set; }
    }
}
