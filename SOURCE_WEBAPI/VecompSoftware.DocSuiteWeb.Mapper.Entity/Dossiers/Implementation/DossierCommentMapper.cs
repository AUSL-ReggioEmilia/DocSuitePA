using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Dossiers
{
    public class DossierCommentMapper : BaseEntityMapper<DossierComment, DossierComment>, IDossierCommentMapper
    {
        public override DossierComment Map(DossierComment entity, DossierComment entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Author = entity.Author;
            entityTransformed.Comment = entity.Comment;
            #endregion

            return entityTransformed;
        }
    }
}
