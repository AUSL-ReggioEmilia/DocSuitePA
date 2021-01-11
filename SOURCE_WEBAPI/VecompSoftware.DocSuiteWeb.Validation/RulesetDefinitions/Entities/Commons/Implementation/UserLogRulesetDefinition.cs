namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons
{
    public class UserLogRulesetDefinition : IUserLogRuleset
    {
        public string READ => "UserLogRead";

        public string INSERT => "UserLogInsert";

        public string UPDATE => "UserLogUpdate";

        public string DELETE => "UserLogDelete";
    }
}
