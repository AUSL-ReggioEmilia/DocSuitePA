using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers
{
    public class DossierCommentValidatorMapper : BaseMapper<DossierComment, DossierCommentValidator>, IDossierCommentValidatorMapper
    {
        public DossierCommentValidatorMapper() { }

        public override DossierCommentValidator Map(DossierComment entity, DossierCommentValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Author = entity.Author;
            entityTransformed.Comment = entity.Comment;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Dossier = entity.Dossier;
            entityTransformed.DossierFolder = entity.DossierFolder;
            #endregion

            return entityTransformed;
        }
    }
}
