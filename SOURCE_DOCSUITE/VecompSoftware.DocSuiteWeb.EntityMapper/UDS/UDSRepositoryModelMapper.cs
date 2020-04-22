using System;
using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.Data.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.UDS
{
    public class UDSRepositoryModelMapper : BaseEntityMapper<UDSRepository, UDSRepositoryModel>
    {
        public UDSRepositoryModelMapper() : base() { }

        public static UDSRepositoryStatus StatusConverter(UDSRepositoryState status)
        {
            switch (status)
            {
                case UDSRepositoryState.Draft:
                    return UDSRepositoryStatus.Draft;
                case UDSRepositoryState.Confirmed:
                    return UDSRepositoryStatus.Confirmed;
                default:
                    return UDSRepositoryStatus.Draft;
            }
        }

        protected override IQueryOver<UDSRepository, UDSRepository> MappingProjection(IQueryOver<UDSRepository, UDSRepository> queryOver)
        {
            UDSRepositoryModel udsModel = null;

            queryOver
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => udsModel.Id)
                    .Select(x => x.ActiveDate).WithAlias(() => udsModel.ActiveDate)
                    .Select(x => x.ExpiredDate).WithAlias(() => udsModel.ExpiredDate)
                    .Select(x => x.ModuleXML).WithAlias(() => udsModel.ModuleXML)
                    .Select(x => x.Name).WithAlias(() => udsModel.Name)
                    .Select(Projections.Cast(
                                NHibernateUtil.Enum(typeof(UDSRepositoryStatus)),
                                Projections.Property("Status")).WithAlias(() => udsModel.Status))
                    .Select(x => x.Version).WithAlias(() => udsModel.Version));

            return queryOver;
        }

        protected override UDSRepositoryModel TransformDTO(UDSRepository entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare UDSRepository se l'entità non è inizializzata");

            UDSRepositoryModel model = new UDSRepositoryModel(entity.Id)
            {
                ActiveDate = entity.ActiveDate,
                ExpiredDate = entity.ExpiredDate,
                ModuleXML = entity.ModuleXML,
                Name = entity.Name,
                Status = StatusConverter(entity.Status),
                Version = entity.Version,
                DSWEnvironment = entity.DSWEnvironment,
                Alias = entity.Alias                
            };

            return model;
        }
    }
}
