using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.PECMails;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.PECMails
{
    public class PECMailLogService : BaseService<PECMailLog>, IPECMailLogService
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public PECMailLogService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IPECMailRuleset pecMailRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, pecMailRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        protected override PECMailLog BeforeCreate(PECMailLog entity)
        {
            if (entity.PECMail != null)
            {
                entity.PECMail = _unitOfWork.Repository<PECMail>().Find(entity.PECMail.EntityId);
            }
            entity.LogDate = DateTime.UtcNow;
            entity.SystemComputer = Environment.MachineName;
            entity.Hash = HashGenerator.GenerateHash(string.Concat(entity.RegistrationUser, "|", entity.SystemComputer, "|", entity.LogType, "|", entity.Description, "|", entity.UniqueId, "|", entity.PECMail.EntityId, "|", entity.LogDate.ToString("yyyyMMddHHmmss")));
            return base.BeforeCreate(entity);
        }
        #endregion

    }
}
