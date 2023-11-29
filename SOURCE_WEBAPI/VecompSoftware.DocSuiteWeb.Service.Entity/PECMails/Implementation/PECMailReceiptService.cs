using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Finder.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.PECMails
{
    public class PECMailReceiptService : BaseService<PECMailReceipt>, IPECMailReceiptService
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Constructor ]

        public PECMailReceiptService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IPECMailRuleset PECMailRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, PECMailRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

		#endregion

		#region [ Methods  ]
		protected override PECMailReceipt BeforeCreate(PECMailReceipt entity)
		{
			if (entity.PECMail != null)
			{
				entity.PECMail = _unitOfWork.Repository<PECMail>().GetByUniqueId(entity.PECMail.UniqueId).SingleOrDefault();
			}
			if (entity.PECMailParent != null)
			{
				entity.PECMailParent = _unitOfWork.Repository<PECMail>().GetByUniqueId(entity.PECMailParent.UniqueId).SingleOrDefault();
			}
			return base.BeforeCreate(entity);
		}
		#endregion

	}
}
