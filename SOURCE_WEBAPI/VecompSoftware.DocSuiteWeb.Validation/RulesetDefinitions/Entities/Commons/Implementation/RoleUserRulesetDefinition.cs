namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons
{
    public class RoleUserRulesetDefinition : IRoleUserRuleset
    {
        public string READ => "RoleUserRead";

        public string INSERT => "RoleUserInsert";

        public string UPDATE => "RoleUserUpdate";

        public string DELETE => "RoleUserDelete";
    }
}
