using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class CollaborationProtocolModel
    {
        #region [ Constructor ]
        public CollaborationProtocolModel()
        {
            ProtocolContacts = new List<ProtocolContactModel>();
            ProtocolContactManuals = new List<ProtocolContactManualModel>();
        }
        #endregion

        #region [ Fields ]

        #endregion

        #region [ Properties ]

        public string Subject { get; set; }

        public string Note { get; set; }

        public CategoryModel Category { get; set; }

        public ContainerModel Container { get; set; }

        public ProtocolTypeModel ProtocolType { get; set; }

        public ICollection<ProtocolContactModel> ProtocolContacts { get; set; }

        public ICollection<ProtocolContactManualModel> ProtocolContactManuals { get; set; }

        #endregion

    }
}
