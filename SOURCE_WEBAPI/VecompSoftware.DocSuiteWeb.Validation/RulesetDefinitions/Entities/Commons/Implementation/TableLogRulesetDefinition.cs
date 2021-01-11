namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons
{
    public class TableLogRulesetDefinition : ITableLogRuleset
    {
        public string READ => "TableLogRead";

        public string INSERT => "TableLogInsert";

        public string UPDATE => "TableLogUpdate";

        public string DELETE => "TableLogDelete";
    }
}
