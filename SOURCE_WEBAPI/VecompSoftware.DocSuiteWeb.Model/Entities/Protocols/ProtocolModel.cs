using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Protocols
{
    public class ProtocolModel
    {
        #region [ Constructor ]
        public ProtocolModel()
        {
            UniqueId = Guid.NewGuid();
            Contacts = new List<ProtocolContactModel>();
            ContactManuals = new List<ProtocolContactManualModel>();
            Attachments = new List<DocumentModel>();
            Annexes = new List<DocumentModel>();
            Roles = new List<ProtocolRoleModel>();
            Users = new List<ProtocolUserModel>();
        }
        #endregion

        #region [ Fields ]

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public short Year { get; set; }

        public int Number { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string DocumentCode { get; set; }

        public string CancelMotivation { get; set; }

        public short IdStatus { get; set; }
        public string Object { get; set; }
        /// <summary>
        /// Subject di AdvancedProtocol
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Trascodifica Code della tabella TableDocType
        /// </summary>
        public string DocumentTypeCode { get; set; }
        public string SDIIdentification { get; set; }
        public int? IdDocument { get; set; }
        public string DocumentProtocol { get; set; }
        public DateTime DocumentDate { get; set; }
        public string Note { get;set; }
        public Guid? IdChainAttachment { get; set; }
        public Guid? IdChainAnnexed { get; set; }
        public TenantAOOModel TenantAOO { get; set; }
        public CategoryModel Category { get; set; }
        public ContainerModel Container { get; set; }
        public ProtocolTypeModel ProtocolType { get; set; }
        public ICollection<ProtocolContactModel> Contacts { get; set; }
        public ICollection<ProtocolContactManualModel> ContactManuals { get; set; }
        public ICollection<ProtocolRoleModel> Roles { get; set; }
        public ICollection<ProtocolUserModel> Users { get; set; }
        public DocumentModel MainDocument { get; set; }
        public ICollection<DocumentModel> Attachments { get; set; }
        public ICollection<DocumentModel> Annexes { get; set; }
        public AdvancedProtocolModel AdvancedProtocols { get; set; }

        #endregion

    }
}
