﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Finder.Processes;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Processes;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Processes;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Processes
{
    public class ProcessesController : BaseODataController<Process, IProcessService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfwork;

        #endregion

        #region [ Constructor ]

        public ProcessesController(IProcessService service, IDataUnitOfWork unitOfWork, ILogger logger, IMapperUnitOfWork mapperUnitOfWork, ISecurity security) 
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapperUnitOfwork = mapperUnitOfWork;
        }

        #endregion

        #region [ Methods ]        

        [HttpGet]
        public IHttpActionResult AvailableProcesses(string name, short? categoryId, Guid? dossierId, bool loadOnlyMy, bool isProcessActive)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<ProcessTableValuedModel> processes =
                    _unitOfWork.Repository<Process>().FindProcesses(Username, Domain, name, categoryId, dossierId, loadOnlyMy, isProcessActive);
                ICollection<ProcessModel> processModels = _mapperUnitOfwork.Repository<IDomainMapper<ProcessTableValuedModel, ProcessModel>>().MapCollection(processes);

                return Ok(processModels);

            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CategoryProcesses(short categoryId, bool loadOnlyMy, int skip, int top)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<ProcessTableValuedModel> processes =
                    _unitOfWork.Repository<Process>().FindCategoryProcesses(Username, Domain, categoryId, loadOnlyMy, skip, top);
                ICollection<ProcessModel> processModels = _mapperUnitOfwork.Repository<IDomainMapper<ProcessTableValuedModel, ProcessModel>>().MapCollection(processes);

                return Ok(processModels);

            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountCategoryProcesses(short categoryId, bool loadOnlyMy)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int processesCount = _unitOfWork.Repository<Process>().CountCategoryProcesses(Username, Domain, categoryId, loadOnlyMy);

                return Ok(processesCount);

            }, _logger, LogCategories);
        }

        #endregion
    }
}