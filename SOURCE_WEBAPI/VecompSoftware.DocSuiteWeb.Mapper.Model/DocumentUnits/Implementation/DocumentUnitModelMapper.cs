using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits
{
    public class DocumentUnitModelMapper : BaseModelMapper<DocumentUnit, DocumentUnitModel>, IDocumentUnitModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public DocumentUnitModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
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

            modelTransformed.TenantAOO = entity.TenantAOO != null ? _mapperUnitOfWork.Repository<IDomainMapper<TenantAOO, TenantAOOModel>>().Map(entity.TenantAOO, new TenantAOOModel()) : null;

            return modelTransformed;
        }

    }
}
