using Newtonsoft.Json;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class MetadataContactValueService : BaseService<MetadataValueContact>, IMetadataContactValueService
    {
        #region [ Field ]
        
        #endregion

        #region [ Constructor ]

        public MetadataContactValueService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IMetadataRepositoryRuleset MetadataRepositoryRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, MetadataRepositoryRuleset, mapperUnitOfWork, security)
        {
            
        }

        #endregion

        #region [ Method ]
        
        #endregion
    }
}
