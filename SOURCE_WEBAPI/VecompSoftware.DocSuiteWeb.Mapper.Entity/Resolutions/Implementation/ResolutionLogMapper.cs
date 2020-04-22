using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Resolutions
{
    public class ResolutionLogMapper : BaseEntityMapper<ResolutionLog, ResolutionLog>, IResolutionLogMapper
    {
        public override ResolutionLog Map(ResolutionLog entity, ResolutionLog entityTransformed)
        {
            #region [ Base 
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

            #endregion

            return entityTransformed;
        }
    }
}
