namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons
{
    public class ContactRulesetDefinition : IContactRuleset
    {
        public string READ => "ContactRead";

        public string INSERT => "ContactInsert";

        public string UPDATE => "ContactUpdate";

        public string DELETE => "ContactDelete";
    }
}
