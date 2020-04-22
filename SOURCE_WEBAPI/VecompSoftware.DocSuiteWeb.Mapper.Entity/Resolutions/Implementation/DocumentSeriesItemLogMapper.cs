using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Resolutions
{
    public class DocumentSeriesItemLogMapper : BaseEntityMapper<DocumentSeriesItemLog, DocumentSeriesItemLog>, IDocumentSeriesItemLogMapper
    {
        public override DocumentSeriesItemLog Map(DocumentSeriesItemLog entity, DocumentSeriesItemLog entityTransformed)
        {
            #region [ Base ]
            //Le proprietà della mappatura sono state commentate per evitare che la procedura di inserimento di un ProtocolLog
            // possa essere soggetta a problemi di sicurezza dei dati 

            //entityTransformed.Year = entity.Year;
            //entityTransformed.Number = entity.Number;
            //entityTransformed.LogDate = entity.LogDate;
            //entityTransformed.SystemComputer = entity.SystemComputer;
            //entityTransformed.Program = entity.Program;
            //entityTransformed.LogType = entity.LogType;
            entityTransformed.LogDescription = entity.LogDescription;
            //entityTransformed.Severity = entity.Severity;
            //entityTransformed.Hash = entity.Hash;

            #endregion

            return entityTransformed;
        }
    }
}
