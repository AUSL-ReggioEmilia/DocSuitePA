
namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Desks
{
    public sealed class DeskRulesetDefinition : IDeskRuleset
    {
        public string READ => "DeskRead";

        public string INSERT => "DeskInsert";

        public string UPDATE => "DeskUpdate";

        public string DELETE => "DeskDelete";
    }
}
