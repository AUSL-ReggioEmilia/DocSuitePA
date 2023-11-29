using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.WebAPI.Common.Configurations;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Collaborations;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Documents.Signs;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Collaborations
{
    public class CollaborationsController : BaseODataController<Collaboration, ICollaborationService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ICollaborationModelMapper _mapper;
        private readonly ICollaborationTableValuedModelMapper _mapperTableValue;

        #endregion

        #region [ Constructor ]

        public CollaborationsController(ICollaborationService service, IDataUnitOfWork unitOfWork, ILogger logger,
            ICollaborationModelMapper mapper, ICollaborationTableValuedModelMapper mapperTableValue, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _mapperTableValue = mapperTableValue;
        }

        #endregion

        #region [ Methods ]

        [HttpPost]
        public IHttpActionResult GetAtVisionSignCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetProposed($"{Domain}\\{Username}", finder);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult CountAtVisionSignCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int countResults = _unitOfWork.Repository<Collaboration>().CountProposed($"{Domain}\\{Username}", finder);
                return Ok(countResults);
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult GetToVisionSignCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetSigning($"{Domain}\\{Username}", finder);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult CountToVisionSignCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int countResults = _unitOfWork.Repository<Collaboration>().CountSigning($"{Domain}\\{Username}", finder);
                return Ok(countResults);
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult GetToVisionDelegateSignCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                string userAccount = $"{Domain}\\{Username}";
                List<string> listDeletagions = new List<string>();
                UserLog userLog = _unitOfWork.Repository<UserLog>().Query(x => x.SystemUser == userAccount, true).SelectAsQueryable().ToList().FirstOrDefault();
                if (userLog != null && !string.IsNullOrEmpty(userLog.UserProfile))
                {
                    userLog.UserProfile = EncryptionHelper.DecryptString(userLog.UserProfile, WebApiConfiguration.PasswordEncryptionKey);
                    DocSuiteWeb.Model.Documents.Signs.UserProfile userProfile = JsonConvert.DeserializeObject<DocSuiteWeb.Model.Documents.Signs.UserProfile>(userLog.UserProfile);
                    foreach (DocSuiteWeb.Model.Documents.Signs.RemoteSignProperty item in userProfile.Value.Values)
                    {
                        if (item.BeenDelegated != null)
                        {
                            foreach (KeyValuePair<string, DelegateUser> contactDelegate in item.BeenDelegated)
                            {
                                if (!listDeletagions.Contains(contactDelegate.Key))
                                {
                                    listDeletagions.Add(contactDelegate.Key);
                                }
                            }
                        }
                    }
                }
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetDelegationSigning(listDeletagions, finder);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult CountToVisionDelegateSignCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                string userAccount = $"{Domain}\\{Username}";
                List<string> listDeletagions = new List<string>();
                UserLog userLog = _unitOfWork.Repository<UserLog>().Query(x => x.SystemUser == userAccount, true).SelectAsQueryable().ToList().FirstOrDefault();
                if (userLog != null && !string.IsNullOrEmpty(userLog.UserProfile))
                {
                    userLog.UserProfile = EncryptionHelper.DecryptString(userLog.UserProfile, WebApiConfiguration.PasswordEncryptionKey);
                    DocSuiteWeb.Model.Documents.Signs.UserProfile userProfile = JsonConvert.DeserializeObject<DocSuiteWeb.Model.Documents.Signs.UserProfile>(userLog.UserProfile);
                    foreach (DocSuiteWeb.Model.Documents.Signs.RemoteSignProperty item in userProfile.Value.Values)
                    {
                        if (item.BeenDelegated != null)
                        {
                            foreach (KeyValuePair<string, DelegateUser> contactDelegate in item.BeenDelegated)
                            {
                                if (!listDeletagions.Contains(contactDelegate.Key))
                                {
                                    listDeletagions.Add(contactDelegate.Key);
                                }
                            }
                        }
                    }
                }
                int countResults = _unitOfWork.Repository<Collaboration>().CountDelegationSigning(listDeletagions, finder);
                return Ok(countResults);
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult GetAtProtocolAdmissionCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetProtocolAdmissions($"{Domain}\\{Username}", finder);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult CountAtProtocolAdmissionCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int countResults = _unitOfWork.Repository<Collaboration>().CountProtocolAdmissions($"{Domain}\\{Username}", finder);
                return Ok(countResults);
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult GetCurrentActivitiesAllCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetAllUsers($"{Domain}\\{Username}", finder);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult CountCurrentActivitiesAllCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int countResults = _unitOfWork.Repository<Collaboration>().CountAllUsers($"{Domain}\\{Username}", finder);
                return Ok(countResults);
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult GetCurrentActivitiesActiveCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<string> signers = _unitOfWork.Repository<RoleUser>().GetAccounts($"{Domain}\\{Username}").Select(s => s.Account).Distinct().ToList();
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetActiveUsers($"{Domain}\\{Username}", signers, finder);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult CountCurrentActivitiesActiveCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<string> signers = _unitOfWork.Repository<RoleUser>().GetAccounts($"{Domain}\\{Username}").Select(s => s.Account).Distinct().ToList();
                int countResults = _unitOfWork.Repository<Collaboration>().CountActiveUsers($"{Domain}\\{Username}", signers, finder);
                return Ok(countResults);
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult GetCurrentActivitiesPastCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetAlreadySigned($"{Domain}\\{Username}", finder);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult CountCurrentActivitiesPastCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int countResults = _unitOfWork.Repository<Collaboration>().CountAlreadySigned($"{Domain}\\{Username}", finder);
                return Ok(countResults);
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult GetToManageCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetManagings($"{Domain}\\{Username}", finder);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult CountToManageCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int countResults = _unitOfWork.Repository<Collaboration>().CountManagings($"{Domain}\\{Username}", finder);
                return Ok(countResults);
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult GetRegisteredCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetRegistered($"{Domain}\\{Username}", finder);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult CountRegisteredCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int countResults = _unitOfWork.Repository<Collaboration>().CountRegistered($"{Domain}\\{Username}", finder);
                return Ok(countResults);
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult GetMyCheckedOutCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetCheckedOuts($"{Domain}\\{Username}", finder);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult CountMyCheckedOutCollaborations(ODataQueryOptions<Collaboration> options, CollaborationFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int countResults = _unitOfWork.Repository<Collaboration>().CountCheckedOuts($"{Domain}\\{Username}", finder);
                return Ok(countResults);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasViewableRight(ODataQueryOptions<Collaboration> options, int idCollaboration)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool checkSecretary = false;
                Collaboration collaboration = _unitOfWork.Repository<Collaboration>().Find(idCollaboration);
                TemplateCollaboration templateCollaboration = _unitOfWork.Repository<TemplateCollaboration>().GetByName(collaboration.TemplateName);
                if (!string.IsNullOrEmpty(templateCollaboration?.JsonParameters))
                {
                    ICollection<JsonParameter> jsonParameters = JsonConvert.DeserializeObject<ICollection<JsonParameter>>(templateCollaboration.JsonParameters);
                    JsonParameter checkSecretaryParameter = jsonParameters.SingleOrDefault(x => x.Name == JsonParameterNames.SECRETARY_VIEW_RIGHT_ENABLED);
                    if (checkSecretaryParameter != null)
                    {
                        checkSecretary = checkSecretaryParameter.ValueBoolean.Value;
                    }
                }
                bool result = _unitOfWork.Repository<Collaboration>().HasViewableRight($"{Domain}\\{Username}", idCollaboration, checkSecretary);
                return Ok(result);
            }, _logger, LogCategories);
        }

        #endregion
    }
}
