namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Tenants
{
    public class TenantAOORuleset : ITenantAOORuleset
    {
        public string READ => "TenantAOORead";

        public string INSERT => "TenantAOOInsert";

        public string UPDATE => "TenantAOOUpdate";

        public string DELETE => "TenantAOODelete";
    }
}
