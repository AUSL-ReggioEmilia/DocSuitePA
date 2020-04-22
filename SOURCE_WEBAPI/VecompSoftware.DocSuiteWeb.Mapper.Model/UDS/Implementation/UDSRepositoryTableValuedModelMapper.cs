using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.UDS
{
    public class UDSRepositoryTableValuedModelMapper : BaseModelMapper<UDSRepositoryTableValuedModel, UDSRepository>, IUDSRepositoryTableValuedModelMapper
    {

        #region [ Fields ]
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion


        #region [ Constructor ]

        public UDSRepositoryTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #endregion

        #region [ Methods ]

        public override UDSRepository Map(UDSRepositoryTableValuedModel model, UDSRepository entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = model.IdUDSRepository;
            entityTransformed.Name = model.Name;
            entityTransformed.Version = model.Version;
            entityTransformed.ActiveDate = model.ActiveDate;
            entityTransformed.ExpiredDate = model.ExpiredDate;
            entityTransformed.RegistrationUser = model.RegistrationUser;
            entityTransformed.RegistrationDate = model.RegistrationDate;
            entityTransformed.LastChangedUser = model.LastChangedUser;
            entityTransformed.LastChangedDate = model.LastChangedDate;
            entityTransformed.Alias = model.Alias;
            entityTransformed.DSWEnvironment = model.DSWEnvironment;
            entityTransformed.SequenceCurrentNumber = model.SequenceCurrentNumber;
            entityTransformed.SequenceCurrentYear = model.SequenceCurrentYear;
            entityTransformed.Name = model.Name;
            entityTransformed.Status = (Entity.UDS.UDSRepositoryStatus)model.Status;
            entityTransformed.Container = _mapperUnitOfWork.Repository<IDomainMapper<IContainerTableValuedModel, Container>>().Map(model, null);
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }

        #endregion

    }
}
