using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits
{
    public class DocumentUnitUDSCommandModelMapper : BaseModelMapper<DocumentUnit, UDSBuildModel>, IDocumentUnitUDSCommandModelMapper
    {

        #region [ Fields ]

        private readonly IMapperUnitOfWork _mapperUnitOfwork;

        #endregion

        #region [ Constructor ]

        public DocumentUnitUDSCommandModelMapper(IMapperUnitOfWork mapperUnitOfwork)
        {
            _mapperUnitOfwork = mapperUnitOfwork;
        }

        #endregion

        #region [ Methods ]


        public override UDSBuildModel Map(DocumentUnit entity, UDSBuildModel entityTransformed)
        {
            #region [ Base ]

            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.Title = entity.Title;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Category = entity.Category != null ? _mapperUnitOfwork.Repository<IDomainMapper<Category, CategoryModel>>().Map(entity.Category, new CategoryModel()) : null;
            entityTransformed.Container = entity.Container != null ? _mapperUnitOfwork.Repository<IDomainMapper<Container, ContainerModel>>().Map(entity.Container, new ContainerModel()) : null;
            entityTransformed.UDSRepository = entity.UDSRepository != null ? _mapperUnitOfwork.Repository<IDomainMapper<UDSRepository, UDSRepositoryModel>>().Map(entity.UDSRepository, new UDSRepositoryModel()) : null;
            entityTransformed.XMLContent = entityTransformed.UDSRepository != null ? entityTransformed.UDSRepository.ModuleXML : null;
            entityTransformed.Roles = _mapperUnitOfwork.Repository<IDomainMapper<DocumentUnitRole, RoleModel>>().MapCollection(entity.DocumentUnitRoles);
            entityTransformed.Documents = _mapperUnitOfwork.Repository<IDomainMapper<DocumentUnitChain, UDSDocumentModel>>().MapCollection(entity.DocumentUnitChains);
            entityTransformed.Users = _mapperUnitOfwork.Repository<IDomainMapper<DocumentUnitUser, UserModel>>().MapCollection(entity.DocumentUnitUsers);
            entityTransformed.Contacts = _mapperUnitOfwork.Repository<IDomainMapper<DocumentUnitContact, UDSContactModel>>().MapCollection(entity.DocumentUnitContacts);
            #endregion

            return entityTransformed;
        }

        #endregion

    }
}
