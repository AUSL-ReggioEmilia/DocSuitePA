using System;
using System.Configuration;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class ContainerService : BaseService<Container>, IContainerService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly ISecurity _security;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        #endregion

        #region [ Constructor ]

        public ContainerService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IContainerRuleset containerRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security, IDecryptedParameterEnvService parameterEnvService)
            : base(unitOfWork, logger, validationService, containerRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _security = security;
            _parameterEnvService = parameterEnvService;
        }

        #endregion

        #region [ Methods ]
        protected override Container BeforeCreate(Container entity)
        {
            if (CurrentInsertActionType == Common.Infrastructures.InsertActionType.InsertContainer)
            {
                entity.UDSLocation = _unitOfWork.Repository<Location>().Find(_parameterEnvService.UDSLocationId);
                entity.AutomaticSecurityGroups = _parameterEnvService.ArchiveSecurityGroupsGenerationEnabled;
                entity.ContainerType = ContainerType.UDS;
                entity.DeskLocation = null;
                entity.DocmLocation = null;
                entity.DocumentSeriesAnnexedLocation = null;
                entity.DocumentSeriesLocation = null;
                entity.DocumentSeriesUnpublishedAnnexedLocation = null;
                entity.HeadingFrontalino = null;
                entity.HeadingLetter = null;
                entity.idArchive = null;
                entity.isActive = true;
                entity.Note = string.Empty;
                entity.ProtAttachLocation = null;
                entity.ProtLocation = null;
                entity.ReslLocation = null;
                if (!string.IsNullOrEmpty(entity.SecurityUserAccount))
                {
                    try
                    {
                        entity.SecurityUserDisplayName = _security.GetUser(entity.SecurityUserAccount).DisplayName;
                    }
                    catch (Exception)
                    {
                        entity.SecurityUserAccount = string.Empty;
                    }
                }
            }

            return base.BeforeCreate(entity);
        }
        #endregion

    }
}