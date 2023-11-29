using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentUnits
{
    public class DocumentUnitContactMapper : BaseEntityMapper<DocumentUnitContact, DocumentUnitContact>, IDocumentUnitContactMapper
    {
        public DocumentUnitContactMapper()
        { }

        public override DocumentUnitContact Map(DocumentUnitContact entity, DocumentUnitContact entityTransformed)
        {
            entityTransformed.ContactLabel = entity.ContactLabel;
            entityTransformed.ContactManual = entity.ContactManual;
            entityTransformed.ContactType = entity.ContactType;
            entityTransformed.UniqueId = entity.UniqueId;

            return entityTransformed;
        }

    }
}
