using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;
using TenantTypologyType = VecompSoftware.DocSuiteWeb.Entity.Tenants.TenantTypologyType;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Tenants
{
    public class TenantTableValuedModelMapper : BaseModelMapper<TenantTableValuedModel, Tenant>, ITenantTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public TenantTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #region [ Methods ]

        public override Tenant Map(TenantTableValuedModel entity, Tenant modelTransformed)
        {
            modelTransformed.UniqueId = entity.IdTenantModel;
            modelTransformed.TenantName = entity.TenantName;
            modelTransformed.CompanyName = entity.CompanyName;
            modelTransformed.StartDate = entity.StartDate;
            modelTransformed.EndDate = entity.EndDate;
            modelTransformed.Note = entity.Note;
            modelTransformed.RegistrationUser = entity.RegistrationUser;
            modelTransformed.RegistrationDate = entity.RegistrationDate;
            modelTransformed.LastChangedUser = entity.LastChangedUser;
            modelTransformed.LastChangedDate = entity.LastChangedDate;
            modelTransformed.Timestamp = entity.Timestamp;
            modelTransformed.TenantTypology = (TenantTypologyType)entity.TenantTypology;

            return modelTransformed;
        }

        public override ICollection<Tenant> MapCollection(ICollection<TenantTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<Tenant>();
            }
            List<Tenant> modelsTransformed = new List<Tenant>();
            Tenant modelTransformed = null;
            foreach (IGrouping<Guid, TenantTableValuedModel> tenantLookup in model.ToLookup(x => x.IdTenantModel))
            {
                modelTransformed = Map(tenantLookup.First(), new Tenant());
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }

        #endregion
    }
}
