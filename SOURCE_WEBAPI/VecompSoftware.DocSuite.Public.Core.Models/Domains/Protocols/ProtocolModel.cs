using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Protocols
{
    [DocumentUnit(DocumentUnitType.Protocol)]
    public class ProtocolModel : DocumentUnitModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]
        public ProtocolModel(Guid id, short year, int number, string subject,
            CategoryModel category, ContainerModel container, string location)
            : base(id, year, number, subject, category, container, location)
        {
            Documents = new HashSet<DocumentModel>();
            Contacts = new HashSet<ProtocolContactModel>();
            Sectors = new HashSet<ProtocolSectorModel>();
            Users = new HashSet<ProtocolUserModel>();
        }
        #endregion

        #region [ Properties ]
        public ProtocolType ProtocolType { get; set; }

        /// <summary>
        /// valore di Subject di AdvancedProtocol, se il protocollo è in entrata
        /// </summary>
        public string Assignee { get; set; }

        /// <summary>
        /// valore  di Subject di AdvancedProtocol, se il protocollo è in uscita
        /// </summary>
        public string Addressee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ServiceCategory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ProtocolStatusType Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AnnulmentReason { get; set; }

        /// <summary>
        /// corrisponde a CheckPublication in Protocol
        /// </summary>
        public string OnlinePublication { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<DocumentModel> Documents { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<ProtocolContactModel> Contacts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<ProtocolSectorModel> Sectors { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<ProtocolUserModel> Users { get; set; }

        #endregion

    }
}
