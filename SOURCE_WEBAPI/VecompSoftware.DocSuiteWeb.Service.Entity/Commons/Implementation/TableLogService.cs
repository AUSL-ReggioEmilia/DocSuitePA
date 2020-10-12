using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class TableLogService : BaseService<TableLog>, ITableLogService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public TableLogService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ITableLogRuleset roleRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, roleRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        public static TableLog CreateLog(Guid? uniqueId, int? entityId, TableLogEvent logType, string logDescription, string tableName, string registrationUser)
        {
            TableLog tableLog = new TableLog()
            {
                LogType = logType,
                LogDescription = logDescription,
                SystemComputer = Environment.MachineName,
                RegistrationDate = DateTimeOffset.UtcNow,
                RegistrationUser = registrationUser,
                TableName = tableName,
                LoggedEntityId = entityId,
                LoggedEntityUniqueId = uniqueId
            };
            tableLog.Hash = HashGenerator.GenerateHash($"{tableLog.RegistrationUser}|{tableLog.LogType}|{tableLog.LogDescription}|{tableLog.UniqueId}|{tableLog.RegistrationDate:yyyyMMddHHmmss}");
            return tableLog;
        }
        #endregion

    }
}