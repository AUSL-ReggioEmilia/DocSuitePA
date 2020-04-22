namespace VecompSoftware.BiblosDS.Model.CQRS.Notifications
{
    public class CommandNotify : CommandModel
    {
        public NotifyLevel NotifyLevel { get; set; }
        public string Message { get; set; }
        public bool Complete { get; set; }
    }
}
