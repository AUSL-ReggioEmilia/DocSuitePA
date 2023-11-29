using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations
{
    public class CollaborationValidatorMapper : BaseMapper<Collaboration, CollaborationValidator>, ICollaborationValidatorMapper
    {
        public CollaborationValidatorMapper() { }


        public override CollaborationValidator Map(Collaboration entity, CollaborationValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.DocumentType = entity.DocumentType;
            entityTransformed.IdPriority = entity.IdPriority;
            entityTransformed.IdStatus = entity.IdStatus;
            entityTransformed.MemorandumDate = entity.MemorandumDate;
            entityTransformed.Note = entity.Note;
            entityTransformed.PublicationDate = entity.PublicationDate;
            entityTransformed.PublicationUser = entity.PublicationUser;
            entityTransformed.RegistrationEmail = entity.RegistrationEmail;
            entityTransformed.RegistrationName = entity.RegistrationName;
            entityTransformed.SignCount = entity.SignCount;
            entityTransformed.SourceProtocolNumber = entity.SourceProtocolNumber;
            entityTransformed.SourceProtocolYear = entity.SourceProtocolYear;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.AlertDate = entity.AlertDate;
            entityTransformed.TemplateName = entity.TemplateName;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.CollaborationLogs = entity.CollaborationLogs;
            entityTransformed.CollaborationSigns = entity.CollaborationSigns;
            entityTransformed.CollaborationUsers = entity.CollaborationUsers;
            entityTransformed.DeskCollaboration = entity.DeskCollaboration;
            entityTransformed.Location = entity.Location;
            entityTransformed.CollaborationVersionings = entity.CollaborationVersionings;
            #endregion

            return entityTransformed;
        }

    }
}
