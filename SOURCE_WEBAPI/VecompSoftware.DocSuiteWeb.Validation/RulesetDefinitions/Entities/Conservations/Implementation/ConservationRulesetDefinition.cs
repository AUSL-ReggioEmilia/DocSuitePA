namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Conservations
{
    public sealed class ConservationRulesetDefinition : IConservationRuleset
    {
        public string READ => "ConservationRead";

        public string INSERT => "ConservationInsert";

        public string UPDATE => "ConservationUpdate";

        public string DELETE => "ConservationDelete";
    }
}
