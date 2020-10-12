using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Protocols
{
    public class ProtocolTableValuedModel : ICategoryTableValuedModel, IContainerTableValuedModel, ITenantAOOTableValuedModel
    {
        #region [ Constructor ]
        public ProtocolTableValuedModel()
        {
            UniqueId = Guid.NewGuid();
        }
        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public short Year { get; set; }

        public int Number { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string DocumentCode { get; set; }

        public short IdStatus { get; set; }

        public string Object { get; set; }

        public int? IdDocument { get; set; }
        public string DocumentProtocol { get; set; }
        public DateTime DocumentDate { get; set; }
        public string Note { get; set; }


        #region [ Category ]

        public short? Category_IdCategory { get; set; }

        public string Category_Name { get; set; }

        #endregion

        #region [ Container ]

        public short? Container_IdContainer { get; set; }

        public string Container_Name { get; set; }

        #endregion

        #region [ ProtocolContact ]

        public int? ProtocolContact_IDContact { get; set; }

        public string ProtocolContact_Description { get; set; }

        #endregion

        #region [ ProtocolContactManual ]

        public int? ProtocolContactManual_Incremental { get; set; }

        public string ProtocolContactManual_Description { get; set; }

        #endregion

        #region [ ProtocolType ]

        public short ProtocolType_IdType { get; set; }

        public string ProtocolType_Description { get; set; }

        #endregion

        #region [ TenantAOO ]
        public Guid? TenantAOO_IdTenantAOO { get; set; }
        public string TenantAOO_Name { get; set; }
        #endregion

        #endregion

    }
}
