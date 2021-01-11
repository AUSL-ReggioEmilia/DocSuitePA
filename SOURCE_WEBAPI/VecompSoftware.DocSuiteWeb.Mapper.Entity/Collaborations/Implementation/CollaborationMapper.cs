using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Collaborations
{
    public class CollaborationMapper : BaseEntityMapper<Collaboration, Collaboration>, ICollaborationMapper
    {
        public override Collaboration Map(Collaboration entity, Collaboration entityTransformed)
        {
            #region [ Base ]
            entityTransformed.AlertDate = entity.AlertDate;
            entityTransformed.DocumentType = entity.DocumentType;
            entityTransformed.IdPriority = entity.IdPriority;
            entityTransformed.IdStatus = entity.IdStatus;
            entityTransformed.MemorandumDate = entity.MemorandumDate;
            entityTransformed.Note = entity.Note;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.PublicationDate = entity.PublicationDate;
            entityTransformed.PublicationUser = entity.PublicationUser;
            entityTransformed.RegistrationEmail = entity.RegistrationEmail;
            entityTransformed.RegistrationName = entity.RegistrationName;
            entityTransformed.SignCount = entity.SignCount;
            entityTransformed.SourceProtocolNumber = entity.SourceProtocolNumber;
            entityTransformed.SourceProtocolYear = entity.SourceProtocolYear;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.TemplateName = entity.TemplateName;
            #endregion

            return entityTransformed;
        }
    }
}
