using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Commons
{
    public class ContactsController : BaseODataController<Contact, IContactService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        private readonly IParameterEnvService _parameterEnvService;

        #endregion

        #region [ Constructor ]

        public ContactsController(IContactService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security, IMapperUnitOfWork mapperUnitOfWork,
            IParameterEnvService parameterEnvService)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapperUnitOfWork = mapperUnitOfWork;
            _parameterEnvService = parameterEnvService;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult FindContacts(ODataQueryOptions<Contact> options, [FromODataUri]ContactFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<ContactTableValuedModel> contacts = _unitOfWork.Repository<Contact>().FindContacts(Username, Domain, finder);
                ICollection<ContactModel> results = _mapperUnitOfWork.Repository<IDomainMapper<ContactTableValuedModel, ContactModel>>().MapCollection(contacts);
                return Ok(results);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetContactParents(ODataQueryOptions<Contact> options, int idContact)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<ContactTableValuedModel> contacts = _unitOfWork.Repository<Contact>().GetContactParents(idContact);
                ICollection<ContactModel> results = _mapperUnitOfWork.Repository<IDomainMapper<ContactTableValuedModel, ContactModel>>().MapCollection(contacts);
                return Ok(results);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetRoleContacts(ODataQueryOptions<Contact> options)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<ContactModel> results = new List<ContactModel>();
                if (!_parameterEnvService.RoleContactEnabled)
                {
                    return Ok(results);
                }

                ICollection<ContactTableValuedModel> contacts = _unitOfWork.Repository<Contact>().GetAuthorizedRoleContacts(Username, Domain, _parameterEnvService.GroupTblContact);
                results = _mapperUnitOfWork.Repository<IDomainMapper<ContactTableValuedModel, ContactModel>>().MapCollection(contacts);
                return Ok(results);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetContactsByParentId(ODataQueryOptions<Contact> options, int idContact)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<Contact> contacts = _unitOfWork.Repository<Contact>().GetContactByParentId(idContact).ToList();
                ICollection<ContactModel> results = _mapperUnitOfWork.Repository<IDomainMapper<Contact, ContactModel>>().MapCollection(contacts);
                return Ok(results);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
