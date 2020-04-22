using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSWorkflowModel
    {
        #region [ Constructor ]

        public UDSWorkflowModel()
        {
            DynamicDatas = new Dictionary<string, string>();
            Roles = new List<UDSRoleModel>();
        }

        #endregion

        #region [ Properties ]

        public ContactModel Contact { get; set; }

        public DocumentUnitModel Referenced { get; set; }

        public FascicleModel Fascicle { get; set; }

        public IDictionary<string, string> DynamicDatas { get; set; }

        public ICollection<UDSRoleModel> Roles { get; set; }

        #endregion
    }
}
