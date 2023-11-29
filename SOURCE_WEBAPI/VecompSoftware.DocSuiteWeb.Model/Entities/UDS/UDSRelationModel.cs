using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSRelationModel
    {
        #region [ Constructor ]
        public UDSRelationModel()
        {
            Users = new List<UDSUserModel>();
            Roles = new List<UDSRoleModel>();
            DocumentUnits = new List<UDSDocumentUnitModel>();
            Contacts = new List<UDSContactModel>();
            PECMails = new List<UDSPECMailModel>();
            Messages = new List<UDSMessageModel>();
            Collaborations = new List<UDSCollaborationModel>();
        }
        #endregion

        #region [ Properties ]

        public UDSRepositoryModel UDSRepository { get; set; }
        public ICollection<UDSUserModel> Users { get; set; }
        public ICollection<UDSRoleModel> Roles { get; set; }
        public ICollection<UDSDocumentUnitModel> DocumentUnits { get; set; }
        public ICollection<UDSContactModel> Contacts { get; set; }
        public ICollection<UDSPECMailModel> PECMails { get; set; }
        public ICollection<UDSMessageModel> Messages { get; set; }
        public ICollection<UDSCollaborationModel> Collaborations { get; set; }

        #endregion
    }
}
