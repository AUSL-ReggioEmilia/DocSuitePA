namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Tenants
{
    public class TenantWorkflowRepositoryRuleset : ITenantWorkflowRepositoryRuleset
    {
        public string READ => "TenantWorkflowRepositoryRead";

        public string INSERT => "TenantWorkflowRepositoryInsert";

        public string UPDATE => "TenantWorkflowRepositoryUpdate";

        public string DELETE => "TenantWorkflowRepositoryDelete";
    }
}
