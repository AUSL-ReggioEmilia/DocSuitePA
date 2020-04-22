using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentUnits
{
    public class DocumentUnitUserMapper : BaseEntityMapper<DocumentUnitUser, DocumentUnitUser>, IDocumentUnitUserMapper
    {
        public DocumentUnitUserMapper()
        { }

        public override DocumentUnitUser Map(DocumentUnitUser entity, DocumentUnitUser entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Account = entity.Account;
            entityTransformed.AuthorizationType = entity.AuthorizationType;
            #endregion

            return entityTransformed;
        }

    }
}
