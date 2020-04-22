using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.DTO.Workflows;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Workflow
{
    public class MapperDematerialisationStatementResult : BaseEntityMapper<Protocol, DematerialisationStatementResult>
    {
        public MapperDematerialisationStatementResult() : base()
        {

        }
        protected override DematerialisationStatementResult TransformDTO(Protocol entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare DematerialisationStatementRequest se l'entità non è inizializzata");
            }
            DematerialisationStatementResult result = new DematerialisationStatementResult();
            result.UDId = entity.UniqueId;
            result.Year = entity.Year;
            //result.UDName = "Protocollo";
            result.Number = entity.Number;
            result.Subject = entity.ProtocolObject;
            result.RegistrationDate = entity.RegistrationDate;
            //result.Title = String.Format("{0}/{1:0000000}", entity.Year, entity.Number);
            result.ContainerName = entity.Container != null ? entity.Container.Name : string.Empty;
           

            return result;
        }

        protected override IQueryOver<Protocol, Protocol> MappingProjection(IQueryOver<Protocol, Protocol> queryOver)
        {
            DematerialisationStatementResult dematerialisationResult = null;
            Container container = null;
           

            queryOver
                    .JoinQueryOver<Container>(o => o.Container, () => container)
                    .SelectList(list => list
                    .Select(x => x.UniqueId).WithAlias(() => dematerialisationResult.UDId)
                    .Select(x => x.Year).WithAlias(() => dematerialisationResult.Year)
                    .Select(x => x.Number).WithAlias(() => dematerialisationResult.Number)
                    .Select(x => x.ProtocolObject).WithAlias(() => dematerialisationResult.Subject)
                    .Select(x => x.RegistrationDate).WithAlias(() => dematerialisationResult.RegistrationDate)
                     .Select(() => container.Name).WithAlias(() => dematerialisationResult.ContainerName)
                    );
            return queryOver;
        }
    }
}
