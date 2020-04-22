namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows.Models
{
    public class ProcurementRightModel
    {
        public string Country { get; set; }
        public string CompanyCode { get; set; }
        public int DocSuiteCeoRole { get; set; }
        public string DocSuiteCeoEmails { get; set; }
        public double LimitEmpoweredResponsibleLCY { get; set; }
        public int DocSuiteProcurementRole { get; set; }
        public string DocSuiteProcurementEmails { get; set; }
        public double LimitProcurementResponsibleLCY { get; set; }
    }
}
