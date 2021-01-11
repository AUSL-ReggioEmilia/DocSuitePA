using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Commons
{
    public class MetadataRepositoriesController : BaseODataController<MetadataRepository, IMetadataRepositoryService>
    {
        #region [ Fields ]
        #endregion

        #region [ Contructor ]
        public MetadataRepositoriesController(IMetadataRepositoryService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
        }
        #endregion

        #region [ Method ]
        #endregion
    }
}