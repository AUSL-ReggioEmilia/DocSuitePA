namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons
{
    public class ContainerRulesetDefinition : IContainerRuleset
    {
        public string READ => "ContainerRead";

        public string INSERT => "ContainerInsert";

        public string UPDATE => "ContainerUpdate";

        public string DELETE => "ContainerDelete";
    }
}
