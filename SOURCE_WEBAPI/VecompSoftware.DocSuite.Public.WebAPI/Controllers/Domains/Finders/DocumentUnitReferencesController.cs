using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.Public.Core.Models.Domains;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Domains.Finders
{
    [EnableQuery]
    public class DocumentUnitReferencesController : BaseODataController<DocumentUnitReferenceModel, Protocol>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        #endregion

        #region [ Constructor ]

        public DocumentUnitReferencesController(IDataUnitOfWork unitOfWork, ILogger logger, IMapper mapper)
            : base(unitOfWork, logger, mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #endregion

        #region [ Methods ]

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult ProtocolByContacts(string searchCode, DateTimeOffset? dateFrom, DateTimeOffset? dateTo)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IQueryable<Protocol> finderResults = _unitOfWork.Repository<Protocol>().GetByContacts(searchCode, dateFrom, dateTo);
                ICollection<DocumentUnitReferenceModel> results = _mapper.Map<ICollection<Protocol>, ICollection<DocumentUnitReferenceModel>>(finderResults.ToList()).ToList();
                return Ok(results);
            }, _logger, LogCategories);
        }

        #endregion
    }
}
