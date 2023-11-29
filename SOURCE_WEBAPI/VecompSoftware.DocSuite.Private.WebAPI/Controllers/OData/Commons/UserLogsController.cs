using Microsoft.AspNet.OData.Query;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuite.WebAPI.Common.Configurations;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Documents.Signs;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Validation;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Commons
{
    public class UserLogsController : BaseODataController<UserLog, IUserLogService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        #endregion

        #region [ Constructor ]

        public UserLogsController(IUserLogService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security, IMapperUnitOfWork mapperUnitOfWork)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #endregion

        #region [ Methods ]

        [HttpGet]
        public IHttpActionResult GetCurrentUserLogSecure(ODataQueryOptions<UserLog> options)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                string systemUser = $"{Domain}\\{Username}";
                UserLog userLog = _unitOfWork.Repository<UserLog>().GetBySystemUser(systemUser);

                if (userLog == null)
                {
                    throw new DSWValidationException("UserLogs validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel() { Key = "UserLog", Message = $"Nessun UserLog per l'utente: {systemUser}" } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }

                if (!string.IsNullOrEmpty(userLog.UserProfile))
                {
                    userLog.UserProfile = EncryptionHelper.DecryptString(userLog.UserProfile, WebApiConfiguration.PasswordEncryptionKey);
                    UserProfile userProfile = JsonConvert.DeserializeObject<UserProfile>(userLog.UserProfile);

                    foreach (KeyValuePair<ProviderSignType, RemoteSignProperty> entry in userProfile.Value)
                    {
                        entry.Value.OTP = string.Empty;
                        entry.Value.Password = string.Empty;
                        entry.Value.PIN = string.Empty;
                    }
                    userLog.UserProfile = JsonConvert.SerializeObject(userProfile);
                }

                return Ok(userLog);
            }, _logger, LogCategories);
        }
        #endregion
    }
}