using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Dossiers;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers
{
    public abstract class BaseDossierService<TEntity> : BaseService<TEntity>
         where TEntity : DSWBaseEntity, new()
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public BaseDossierService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationservice,
            IDossierRuleset dossierRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationservice, dossierRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        public static DossierLog CreateLog(Dossier dossier, DossierFolder dossierFolder, DossierLogType logType, string logDescription, string registrationUser)
        {
            DossierLog dossierLog = new DossierLog()
            {
                LogType = logType,
                LogDescription = logDescription,
                SystemComputer = Environment.MachineName,
                RegistrationDate = DateTimeOffset.UtcNow,
                RegistrationUser = registrationUser,
                Entity = dossier,
                DossierFolder = dossierFolder
            };
            dossierLog.Hash = HashGenerator.GenerateHash($"{dossierLog.RegistrationUser}|{dossierLog.LogType}|{dossierLog.LogDescription}|{dossierLog.UniqueId}|{dossier.UniqueId}|{dossierLog.RegistrationDate:yyyyMMddHHmmss}");
            return dossierLog;
        }

        #endregion
    }
}
