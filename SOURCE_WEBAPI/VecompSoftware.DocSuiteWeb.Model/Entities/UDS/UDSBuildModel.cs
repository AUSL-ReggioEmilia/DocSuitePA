using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSBuildModel : IWorkflowContentBase
    {
        #region [ Constructor ]

        public UDSBuildModel()
        {
            WorkflowActions = new List<IWorkflowAction>();
            Documents = new List<UDSDocumentModel>();
            Roles = new List<RoleModel>();
            Users = new List<UserModel>();
        }

        public UDSBuildModel(string xmlContent) : this()
        {
            XMLContent = xmlContent;
        }

        #endregion

        #region [ Properties ]
        public string XMLContent { get; set; }
        public DateTimeOffset? ActiveDate { get; set; }
        public Guid UniqueId { get; set; }
        public Guid? UDSId { get; set; }
        public bool WorkflowAutoComplete { get; set; }
        public string WorkflowName { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        public string RegistrationUser { get; set; }
        public short? Year { get; set; }
        public int? Number { get; set; }
        public string Subject { get; set; }
        public string Title { get; set; }
        public string CancelMotivation { get; set; }
        public DateTimeOffset? RegistrationDate { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string LastChangedUser { get; set; }
        #endregion

        #region[ Navigation Properties ]
        public UDSRepositoryModel UDSRepository { get; set; }
        public CategoryModel Category { get; set; }
        public ContainerModel Container { get; set; }
        public ICollection<RoleModel> Roles { get; set; }
        public ICollection<UserModel> Users { get; set; }
        public ICollection<UDSDocumentModel> Documents { get; set; }
        public ICollection<IWorkflowAction> WorkflowActions { get; set; }
        #endregion

        #region [ Methods ]

        public bool HasRepository()
        {
            return UDSRepository != null;
        }

        #endregion
    }
}
