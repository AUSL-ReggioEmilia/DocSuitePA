
namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.ServiceBus
{
    public class QueueRuleset : IQueueRuleset
    {
        public string READ => "QueueRead";

        public string INSERT => "QueueInsert";

        public string UPDATE => "QueueUpdate";

        public string DELETE => "QueueDelete";
    }
}
