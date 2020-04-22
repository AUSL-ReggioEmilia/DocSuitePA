using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.DTO.Desks;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Desks
{
    public class MapperDeskResult : BaseEntityMapper<Desk, DeskResult>
    {
        public MapperDeskResult() : base()
        {

        }
        protected override DeskResult TransformDTO(Desk entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare DeskResult se l'entità non è inizializzata");
            }
            DeskResult result = new DeskResult();
            result.DeskId = entity.Id;
            result.DeskName = entity.Name;
            result.DeskSubject = entity.Description;
            result.DeskState = entity.Status.Value;
            result.DeskExpirationDate = entity.ExpirationDate;
            result.ContainerName = entity.Container.Name;
            return result;
        }

        protected override IQueryOver<Desk, Desk> MappingProjection(IQueryOver<Desk, Desk> queryOver)
        {
            DeskResult deskResult = null;
            Container container = null;
            
            queryOver
                .JoinQueryOver<Container>(o => o.Container, () => container)
                .SelectList(list => list
                    // Mappatura degli oggetti Desk
                    .Select(x => x.Id).WithAlias(() => deskResult.DeskId)
                    .Select(x => x.Name).WithAlias(() => deskResult.DeskName)
                    .Select(x => x.Description).WithAlias(() => deskResult.DeskSubject)
                    .Select(x => x.Status).WithAlias(() => deskResult.DeskState)
                    .Select(x => x.ExpirationDate).WithAlias(() => deskResult.DeskExpirationDate)
                    // Mappatura degli oggetti Container
                    .Select(() => container.Name).WithAlias(() => deskResult.ContainerName));

            return queryOver;
        }
    }
}
