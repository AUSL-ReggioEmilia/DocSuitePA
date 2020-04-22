using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits
{
    public class DocumentUnitModelMapper : BaseModelMapper<DocumentUnit, DocumentUnitModel>, IDocumentUnitModelMapper
    {

        public DocumentUnitModelMapper()
        {
        }

        public override DocumentUnitModel Map(DocumentUnit entity, DocumentUnitModel modelTransformed)
        {
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.EntityId = entity.EntityId;
            modelTransformed.Environment = entity.Environment;
            modelTransformed.DocumentUnitName = entity.DocumentUnitName;
            modelTransformed.Year = entity.Year;
            modelTransformed.Number = entity.Number.ToString();
            modelTransformed.Title = entity.Title;
            modelTransformed.RegistrationDate = entity.RegistrationDate;
            modelTransformed.RegistrationUser = entity.RegistrationUser;
            modelTransformed.Subject = entity.Subject;

            return modelTransformed;
        }

    }
}
