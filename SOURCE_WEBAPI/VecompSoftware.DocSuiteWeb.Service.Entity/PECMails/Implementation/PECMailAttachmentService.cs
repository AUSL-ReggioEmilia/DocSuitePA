using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.PECMails
{
    public class PECMailAttachmentService : BaseService<PECMailAttachment>, IPECMailAttachmentService
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Constructor ]

        public PECMailAttachmentService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IPECMailAttachmentRuleset pecMailAttachmentRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, pecMailAttachmentRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override PECMailAttachment BeforeCreate(PECMailAttachment entity)
        {
            if (entity.PECMail != null)
            {
                entity.PECMail = _unitOfWork.Repository<PECMail>().Find(entity.PECMail.EntityId);
            }

            return base.BeforeCreate(entity);
        }

        protected override PECMailAttachment BeforeUpdate(PECMailAttachment entity, PECMailAttachment entityTransformed)
        {
            if (entity.PECMail != null)
            {
                entityTransformed.PECMail = _unitOfWork.Repository<PECMail>().Find(entity.PECMail.EntityId);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion

    }
}
