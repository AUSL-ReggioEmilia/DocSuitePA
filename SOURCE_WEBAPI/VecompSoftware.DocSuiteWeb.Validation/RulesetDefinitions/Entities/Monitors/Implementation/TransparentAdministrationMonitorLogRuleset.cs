namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Monitors
{
    public sealed class TransparentAdministrationMonitorLogRuleset : ITransparentAdministrationMonitorLogRuleset
    {
        public string READ => "TransparentAdministrationMonitorLogRead";

        public string INSERT => "TransparentAdministrationMonitorLogInsert";

        public string UPDATE => "TransparentAdministrationMonitorLogUpdate";

        public string DELETE => "TransparentAdministrationMonitorLogDelete";
    }
}
