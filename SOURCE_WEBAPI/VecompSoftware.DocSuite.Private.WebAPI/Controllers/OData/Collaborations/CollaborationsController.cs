using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
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

        [HttpGet]
        public IHttpActionResult GetAtVisionSignCollaborations(ODataQueryOptions<Collaboration> options)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetProposed($"{Domain}\\{Username}");
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetToVisionSignCollaborations(ODataQueryOptions<Collaboration> options, bool? isRequired)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetSigning($"{Domain}\\{Username}", isRequired);

                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetToVisionDelegateSignCollaborations(ODataQueryOptions<Collaboration> options)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                string userAccount = $"{Domain}\\{Username}";
                List<string> listDeletagions = new List<string>();
                UserLog userLog = _unitOfWork.Repository<UserLog>().Query(x => x.SystemUser == userAccount, true).SelectAsQueryable().ToList().FirstOrDefault();
                if (userLog != null && !string.IsNullOrEmpty(userLog.UserProfile))
                {
                    DocSuiteWeb.Model.Documents.Signs.UserProfile userProfile = JsonConvert.DeserializeObject<DocSuiteWeb.Model.Documents.Signs.UserProfile>(userLog.UserProfile);
                    foreach (DocSuiteWeb.Model.Documents.Signs.RemoteSignProperty item in userProfile.Value.Values)
                    {
                        if (item.BeenDelegated != null)
                        {                             
                            foreach (KeyValuePair<string, DelegateUser>   contactDelegate in item.BeenDelegated)
                            {
                                if (!listDeletagions.Contains(contactDelegate.Key))
                                {
                                    listDeletagions.Add(contactDelegate.Key);
                                }
                            }
                        }
                    }
                }
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetDelegationSigning(listDeletagions);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAtProtocolAdmissionCollaborations(ODataQueryOptions<Collaboration> options)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetProtocolAdmissions($"{Domain}\\{Username}");
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetCurrentActivitiesAllCollaborations(ODataQueryOptions<Collaboration> options)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetAllUsers($"{Domain}\\{Username}");
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetCurrentActivitiesActiveCollaborations(ODataQueryOptions<Collaboration> options)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<string> signers = _unitOfWork.Repository<RoleUser>().GetAccounts($"{Domain}\\{Username}").Select(s => s.Account).Distinct().ToList();
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetActiveUsers($"{Domain}\\{Username}", signers);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetCurrentActivitiesPastCollaborations(ODataQueryOptions<Collaboration> options)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetAlreadySigned($"{Domain}\\{Username}");
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetToManageCollaborations(ODataQueryOptions<Collaboration> options)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetManagings($"{Domain}\\{Username}");
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        [EnableQuery]
        public IHttpActionResult GetRegisteredCollaborations(ODataQueryOptions<Collaboration> options, string dateFrom, string dateTo)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetRegistered($"{Domain}\\{Username}",
                    DateTimeOffset.ParseExact(dateFrom, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                    DateTimeOffset.ParseExact(dateTo, "yyyyMMddHHmmss", CultureInfo.InvariantCulture));
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetMyCheckedOutCollaborations(ODataQueryOptions<Collaboration> options)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CollaborationTableValuedModel> results = _unitOfWork.Repository<Collaboration>().GetCheckedOuts($"{Domain}\\{Username}");
                return Ok(_mapperTableValue.MapCollection(results));
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
