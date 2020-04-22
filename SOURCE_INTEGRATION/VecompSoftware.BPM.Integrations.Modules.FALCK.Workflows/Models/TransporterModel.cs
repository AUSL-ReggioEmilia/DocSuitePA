using VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Entities;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows.Models
{
    public class TransporterModel
    {
        public UDSRepository UDSRepository { get; set; }
        public UDSBaseEntity UDS { get; set; }
        public WorkflowMetadata WorkflowMetadata { get; set; }
        public CollaborationModel CollaborationModel { get; set; }
        public DomainUserModel DomainUser { get; set; }
        public Fascicle Fascicle { get; set; }
        public SpecialCategoryModel SpecialCategory { get; set; }
        public ProcurementRightModel ProcurementRight { get; set; }
    }

}
