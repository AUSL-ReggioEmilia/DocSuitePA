using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Resolutions
{
    public class ResolutionMapper : BaseEntityMapper<Resolution, Resolution>, IResolutionMapper
    {
        public override Resolution Map(Resolution entity, Resolution entityTransformed)
        {
            #region [ Base ]
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
            entityTransformed.Status = entity.Status;
            entityTransformed.InclusiveNumber = entity.InclusiveNumber;
            entityTransformed.WebPublicationDate = entity.WebPublicationDate;
            entityTransformed.Amount = entity.Amount;
            #endregion

            return entityTransformed;
        }
    }
}
