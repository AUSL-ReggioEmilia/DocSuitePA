
namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Tenants
{
    public class TenantRulesetDefinition : ITenantRuleset
    {
        public string READ => "TenantRead";

        public string INSERT => "TenantInsert";

        public string UPDATE => "TenantUpdate";

        public string DELETE => "TenantDelete";
    }
}
