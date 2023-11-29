using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions
{
    public class ResolutionValidatorMapper : BaseMapper<Resolution, ResolutionValidator>, IResolutionValidatorMapper
    {
        public ResolutionValidatorMapper() { }

        public override ResolutionValidator Map(Resolution entity, ResolutionValidator entityTransformed)
        {

            entityTransformed.Number = entity.Number;
            entityTransformed.Year = entity.Year;
            entityTransformed.Object = entity.Object;
            entityTransformed.ServiceNumber = entity.ServiceNumber;
            entityTransformed.AdoptionDate = entity.AdoptionDate;
            entityTransformed.AlternativeAssignee = entity.AlternativeAssignee;
            entityTransformed.AlternativeManager = entity.AlternativeManager;
            entityTransformed.AlternativeProposer = entity.AlternativeProposer;
            entityTransformed.AlternativeRecipient = entity.AlternativeRecipient;
            entityTransformed.ConfirmDate = entity.ConfirmDate;
            entityTransformed.EffectivenessDate = entity.EffectivenessDate;
            entityTransformed.LeaveDate = entity.LeaveDate;
            entityTransformed.Number = entity.Number;
            entityTransformed.ProposeDate = entity.ProposeDate;
            entityTransformed.PublishingDate = entity.PublishingDate;
            entityTransformed.ResponseDate = entity.ResponseDate;
            entityTransformed.WaitDate = entity.WaitDate;
            entityTransformed.WarningDate = entity.WarningDate;
            entityTransformed.WorkflowType = entity.WorkflowType;
            entityTransformed.ProposeUser = entity.ProposeUser;
            entityTransformed.LeaveUser = entity.LeaveUser;
            entityTransformed.EffectivenessUser = entity.EffectivenessUser;
            entityTransformed.ResponseUser = entity.ResponseUser;
            entityTransformed.WaitUser = entity.WaitUser;
            entityTransformed.ConfirmUser = entity.ConfirmUser;
            entityTransformed.WarningUser = entity.WarningUser;
            entityTransformed.PublishingUser = entity.PublishingUser;
            entityTransformed.AdoptionUser = entity.AdoptionUser;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Status = entity.Status;
            entityTransformed.InclusiveNumber = entity.InclusiveNumber;
            entityTransformed.WebPublicationDate = entity.WebPublicationDate;
            entityTransformed.Amount = entity.Amount;

            #region [ Navigation Properties ]

            entityTransformed.FileResolution = entity.FileResolution;
            entityTransformed.Category = entity.Category;
            entityTransformed.Container = entity.Container;
            entityTransformed.ResolutionContacts = entity.ResolutionContacts;
            entityTransformed.ResolutionLogs = entity.ResolutionLogs;
            #endregion

            return entityTransformed;
        }

    }
}
