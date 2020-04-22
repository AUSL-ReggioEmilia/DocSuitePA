using VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class DocumentUnitChainTableValuedModelMapper : BaseModelMapper<DocumentUnitTableValuedModel, DocumentUnitChainModel>, IDocumentUnitChainTableValuedModelMapper
    {
        public override DocumentUnitChainModel Map(DocumentUnitTableValuedModel model, DocumentUnitChainModel modelTransformed)
        {
            modelTransformed = null;
            if (!string.IsNullOrEmpty(model.DocumentUnitChain_DocumentName))
            {
                modelTransformed = new DocumentUnitChainModel
                {
                    DocumentName = model.DocumentUnitChain_DocumentName
                };
            }
            return modelTransformed;
        }

    }
}
