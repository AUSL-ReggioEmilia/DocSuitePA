using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits
{
    public class DocumentUnitCollaborationTableValueModelMapper : BaseModelMapper<CollaborationTableValuedModel, DocumentUnitModel>, IDocumentUnitCollaborationTableValueModelMapper
    {
        public override DocumentUnitModel Map(CollaborationTableValuedModel entity, DocumentUnitModel entityTransformed)
        {
            entityTransformed = null;
            if (entity.DocumentUnit_IdDocumentUnit.HasValue)
            {
                entityTransformed = new DocumentUnitModel
                {
                    UniqueId = entity.DocumentUnit_IdDocumentUnit.Value,
                    DocumentUnitName = entity.DocumentUnit_DocumentUnitName,
                    EntityId = entity.DocumentUnit_EntityId,
                    RegistrationDate = entity.DocumentUnit_RegistrationDate,
                    Title = entity.DocumentUnit_Title,
                    Year = entity.DocumentUnit_Year ?? 0,
                    Number = entity.DocumentUnit_Number.HasValue ? entity.DocumentUnit_Number.Value.ToString() : string.Empty,
                    IdUDSRepository = entity.DocumentUnit_IdUDSRepository,
                    Environment = entity.DocumentUnit_Environment.Value
                };
            }

            return entityTransformed;
        }
    }
}
