
namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Tenants
{
    public class TenantConfigurationRulesetDefinition : ITenantConfigurationRuleset
    {
        public string READ => "TenantConfigurationRead";

        public string INSERT => "TenantConfigurationInsert";

        public string UPDATE => "TenantConfigurationUpdate";

        public string DELETE => "TenantConfigurationDelete";
    }
}
