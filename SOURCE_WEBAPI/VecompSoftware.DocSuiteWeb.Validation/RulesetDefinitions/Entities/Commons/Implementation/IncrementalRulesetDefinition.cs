namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons
{
    public class IncrementalRulesetDefinition : IIncrementalRuleset
    {
        public string READ => "IncrementalRead";

        public string INSERT => "IncrementalInsert";

        public string UPDATE => "IncrementalUpdate";

        public string DELETE => "IncrementalDelete";
    }
}
