namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Protocols
{
    public class ProtocolRulesetDefinition : IProtocolRuleset
    {
        public string READ => "ProtocolRead";

        public string INSERT => "ProtocolInsert";

        public string UPDATE => "ProtocolUpdate";

        public string DELETE => "ProtocolDelete";
    }
}
