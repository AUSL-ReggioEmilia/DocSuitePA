using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions;
using VecompSoftware.DocSuite.WebAPI.Common.Configurations;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Finder.Resolutions;
using PrivateResolutionModels = VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Domains.Resolutions
{
    //TODO: Il controller è senza s perchè se si usa la dicitura 'Resolutions', la chiamata va in errore. 
    //      Analizzato dai membri del team e dal responsabile, rimane un mistero.

    [EnableQuery]
    public class ResolutionController : BaseODataController<ResolutionModel, Resolution>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        #endregion

        #region [ Constructor ]

        public ResolutionController(IDataUnitOfWork unitOfWork, ILogger logger, IMapper mapper)
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
        public IHttpActionResult GetExecutiveResolutions(int skip, int top, ResolutionType type, short? year, int? number, string subject, string adoptionDate, string proposer)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                byte idType = Convert.ToByte(type);
                IList<Resolution> resolutions = _unitOfWork.Repository<Resolution>().GetExecutiveByType(skip, top, idType, year, number, subject, adoptionDate, proposer).ToList();
                if (resolutions == null)
                {
                    throw new ArgumentNullException("Resolution not found");
                }

                IList<ResolutionModel> models = _mapper.Map<ICollection<Resolution>, ICollection<ResolutionModel>>(resolutions).ToList();

                return Ok(models.AsQueryable());
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetPublishedResolutions(int skip, int top, ResolutionType type, short? year, int? number, string subject, string adoptionDate, string proposer)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                byte idType = Convert.ToByte(type);
                IList<Resolution> resolutions = _unitOfWork.Repository<Resolution>().GetPublishedByType(skip, top, idType, year, number, subject, adoptionDate, proposer).ToList();
                if (resolutions == null)
                {
                    throw new ArgumentNullException("Resolution not found");
                }

                IList<ResolutionModel> models = _mapper.Map<ICollection<Resolution>, ICollection<ResolutionModel>>(resolutions).ToList();

                return Ok(models.AsQueryable());
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 300, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetOnlinePublishedResolutions(ResolutionType type)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                short onlinePublicationInterval = WebApiConfiguration.OnlinePublicationInterval;
                byte idType = Convert.ToByte(type);
                ICollection<PrivateResolutionModels.ResolutionTableValuedModel> resolutions = _unitOfWork.Repository<Resolution>().GetOnlinePublishedByType(idType, onlinePublicationInterval);
                if (resolutions == null)
                {
                    throw new ArgumentNullException("Resolutions not found");
                }
                ICollection<ResolutionModel> models = _mapper.Map<ICollection<PrivateResolutionModels.ResolutionTableValuedModel>, ICollection<ResolutionModel>>(resolutions).ToList();

                return Ok(models.AsQueryable());
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetExecutiveResolutionsCount(ResolutionType type, short? year, int? number, string subject, string adoptionDate, string proposer)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                byte idType = Convert.ToByte(type);
                long count = _unitOfWork.Repository<Resolution>().CountExecutiveByType(idType, year, number, subject, adoptionDate, proposer);
                return Ok(count);
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetPublishedResolutionsCount(ResolutionType type, short? year, int? number, string subject, string adoptionDate, string proposer)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                byte idType = Convert.ToByte(type);
                long count = _unitOfWork.Repository<Resolution>().CountPublishedByType(idType, year, number, subject, adoptionDate, proposer);
                return Ok(count);
            }, _logger, LogCategories);
        }


        #endregion
    }
}
