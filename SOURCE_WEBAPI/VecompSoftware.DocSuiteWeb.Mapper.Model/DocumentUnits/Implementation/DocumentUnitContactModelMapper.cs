using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits
{
    public class DocumentUnitContactModelMapper : BaseModelMapper<DocumentUnitContact, DocumentUnitContactModel>, IDocumentUnitContactModelMapper
    {

        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public DocumentUnitContactModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
        }
        #endregion

        public override DocumentUnitContactModel Map(DocumentUnitContact entity, DocumentUnitContactModel entityTransformed)
        {
            entityTransformed.ContactLabel = entity.ContactLabel;
            entityTransformed.ContactManual = entity.ContactManual;
            entityTransformed.ContactType = entity.ContactType;
            entityTransformed.UniqueId = entity.UniqueId;

            return entityTransformed;
        }
    }
}
