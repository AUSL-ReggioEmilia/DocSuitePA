using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions
{
    public class ResolutionModel : IWorkflowContentBase
    {
        #region [ Constructor ]
        public ResolutionModel()
        {

        }

        public ResolutionModel(int idResolution)
        {
            UniqueId = Guid.NewGuid();
            IdResolution = idResolution;
            WorkflowActions = new List<IWorkflowAction>();
        }

        #endregion

        #region [ Properties ]

        public int? IdResolution { get; set; }

        public Guid UniqueId { get; set; }
        public bool WorkflowAutoComplete { get; set; }

        public string WorkflowName { get; set; }

        public Guid? IdWorkflowActivity { get; set; }

        public string DocumentUnitName { get; set; }

        public string RegistrationUser { get; set; }

        public DateTime? AdoptionDate { get; set; }

        public string AlternativeAssignee { get; set; }

        public string AlternativeManager { get; set; }

        public string AlternativeProposer { get; set; }

        public string AlternativeRecipient { get; set; }

        public CategoryModel Category { get; set; }

        public ContainerModel Container { get; set; }

        public DateTime? ConfirmDate { get; set; }

        public DateTime? EffectivenessDate { get; set; }

        public DateTime? LeaveDate { get; set; }

        public int? Number { get; set; }

        public DateTime? ProposeDate { get; set; }

        public DateTime? PublishingDate { get; set; }

        public DateTime? ResponseDate { get; set; }

        public string ServiceNumber { get; set; }

        public string Subject { get; set; }

        public DateTime? WaitDate { get; set; }

        public DateTime? WarningDate { get; set; }

        public string WorkflowType { get; set; }

        public short? Year { get; set; }

        public string ProposeUser { get; set; }

        public string LeaveUser { get; set; }

        public string EffectivenessUser { get; set; }

        public string ResponseUser { get; set; }

        public string WaitUser { get; set; }

        public string ConfirmUser { get; set; }

        public string WarningUser { get; set; }

        public string PublishingUser { get; set; }

        public string AdoptionUser { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public FileResolutionModel FileResolution { get; set; }

        public ICollection<ResolutionRoleModel> ResolutionRoles { get; set; }

        public ICollection<IWorkflowAction> WorkflowActions { get; set; }
        #endregion

        #region [ Methods ]

        public bool HasId()
        {
            return IdResolution.HasValue && !IdResolution.Equals(0);
        }

        #endregion
    }
}
