namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons
{
    public class RoleRulesetDefinition : IRoleRuleset
    {
        public string READ => "RoleRead";

        public string INSERT => "RoleInsert";

        public string UPDATE => "RoleUpdate";

        public string DELETE => "RoleDelete";
    }
}
