using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Mappers
{
    public class DocumentModelMapper
    {
        public static DocumentModel Map(DocumentUnitChain documentUnitChain, ArchiveDocument archiveDocument)
        {
            return new DocumentModel
            {
                DocumentId = archiveDocument.IdDocument,
                FileName = archiveDocument.Name,
                ChainType = (DocSuiteWeb.Model.Entities.DocumentUnits.ChainType)documentUnitChain.ChainType,
                ChainId = archiveDocument.IdChain.Value,
                ArchiveSection = documentUnitChain.DocumentLabel
            };
        }
    }
}
