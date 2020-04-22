namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.ServiceBus
{
    public class TopicRuleset : ITopicRuleset
    {
        public string READ => "TopicRead";

        public string INSERT => "TopicInsert";

        public string UPDATE => "TopicUpdate";

        public string DELETE => "TopicDelete";
    }
}
