namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons
{
    public class PrivacyLevelRulesetDefinition : IPrivacyLevelRuleset
    {
        public string READ => "PrivacyLevelRead";

        public string INSERT => "PrivacyLevelInsert";

        public string UPDATE => "PrivacyLevelUpdate";

        public string DELETE => "PrivacyLevelDelete";
    }
}
