using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Templates
{
    public class TemplateDocumentRepositoryService : BaseService<TemplateDocumentRepository>, ITemplateDocumentRepositoryService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public TemplateDocumentRepositoryService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ITemplateDocumentRepositoryRuleset templateDocumentRepositoryRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, templateDocumentRepositoryRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        protected override TemplateDocumentRepository BeforeCreate(TemplateDocumentRepository entity)
        {
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.INSERT, string.Concat("Inserimento deposito documentale ", entity.Name), typeof(TemplateDocumentRepository).Name, CurrentDomainUser.Account));
            return base.BeforeCreate(entity);
        }

        protected override TemplateDocumentRepository BeforeUpdate(TemplateDocumentRepository entity, TemplateDocumentRepository entityTransformed)
        {
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.UPDATE, string.Concat("Modificato deposito documentale ", entity.Name), typeof(TemplateDocumentRepository).Name, CurrentDomainUser.Account));
            return base.BeforeUpdate(entity, entityTransformed);

        }

    }

}



#endregion
