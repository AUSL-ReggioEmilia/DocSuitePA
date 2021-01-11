
namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentUnits
{
    public sealed class DocumentUnitRulesetDefinition : IDocumentUnitRuleset
    {
        public string READ => "DocumentUnitRead";

        public string INSERT => "DocumentUnitInsert";

        public string UPDATE => "DocumentUnitUpdate";

        public string DELETE => "DocumentUnitDelete";
    }
}
