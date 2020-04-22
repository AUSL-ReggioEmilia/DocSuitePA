using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Processes;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Processes;
using VecompSoftware.DocSuiteWeb.Model.Processes;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Dossiers
{
    //la doppia s è voluta ed è dovuta ad un incorretto funzionamento del controller
    //DossierFolders, l'unico modo per ottenere un corretto funzionamento è aggiungere lettere o
    //comunque non usare la dicitura esatta DossierFolders
    public class DossierFolderssController : BaseODataController<DossierFolder, IDossierFolderService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfwork;
        #endregion

        #region [ Constructor ]

        public DossierFolderssController(IDossierFolderService service, IDataUnitOfWork unitOfWork, ILogger logger, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapperUnitOfwork = mapperUnitOfWork;
        }

        #endregion

        #region [ Methods ]

        [HttpGet]
        public IHttpActionResult GetRootDossierFolders(ODataQueryOptions<DossierFolder> options, Guid idDossier, short? status)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<DossierFolderTableValuedModel> dossierFolders = _unitOfWork.Repository<DossierFolder>().GetRootDossierFolders(idDossier, status);
                ICollection<DossierFolderModel> dossierFoldersModel = _mapperUnitOfwork.Repository<IDomainMapper<DossierFolderTableValuedModel, DossierFolderModel>>().MapCollection(dossierFolders).ToList();
                return Ok(dossierFoldersModel);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetChildrenByParent(ODataQueryOptions<DossierFolder> options, Guid idDossierFolder, short? status)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<DossierFolderTableValuedModel> dossierFolders = _unitOfWork.Repository<DossierFolder>().GetChildrenByParent(idDossierFolder, status);
                ICollection<DossierFolderModel> dossierFoldersModel = _mapperUnitOfwork.Repository<IDomainMapper<DossierFolderTableValuedModel, DossierFolderModel>>().MapCollection(dossierFolders).ToList();
                return Ok(dossierFoldersModel);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetProcessFolders(string name, Guid idProcess, bool loadOnlyActive, bool loadOnlyMy)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<DossierFolderTableValuedModel> dossierFoldersModel = _unitOfWork.Repository<DossierFolder>().FindProcessFolders(Username, Domain, name, idProcess, loadOnlyActive, loadOnlyMy);

                ICollection<DossierFolderModel> dossierFolderModel = _mapperUnitOfwork.Repository<IDomainMapper<DossierFolderTableValuedModel, DossierFolderModel>>().MapCollection(dossierFoldersModel).ToList();
                dossierFolderModel = MapDossierFoldersRecursive(dossierFolderModel);

                return Ok(dossierFolderModel);
            }, _logger, LogCategories);
        }

        #region [ Helpers ]

        private ICollection<DossierFolderModel> MapDossierFoldersRecursive(ICollection<DossierFolderModel> dossierFoldersModel)
        {
            foreach (DossierFolderModel dossierFolderModel in dossierFoldersModel)
            {
                ICollection<ProcessFascicleTemplate> fascicleTemplates = _unitOfWork.Repository<ProcessFascicleTemplate>().FindFascicleTemplatesByIdDossierFolder(dossierFolderModel.UniqueId).ToList();
                if (fascicleTemplates.Count != 0)
                {
                    dossierFolderModel.FascicleTemplates = _mapperUnitOfwork.Repository<IDomainMapper<ProcessFascicleTemplate, ProcessFascicleTemplateModel>>().MapCollection(fascicleTemplates).ToList();
                }

                ICollection<DossierFolderTableValuedModel> dossierFolders = _unitOfWork.Repository<DossierFolder>().GetChildrenByParent(dossierFolderModel.UniqueId, 0);
                ICollection<DossierFolderModel> childDossiers = _mapperUnitOfwork.Repository<IDomainMapper<DossierFolderTableValuedModel, DossierFolderModel>>().MapCollection(dossierFolders).ToList();

                //exclude parent dossier from list
                if (dossierFolderModel.DossierFolderLevel == 1 && childDossiers.Count != 0)
                {
                    dossierFoldersModel = MapDossierFoldersRecursive(childDossiers);
                    continue;
                }

                if (childDossiers.Count != 0)
                {
                    dossierFolderModel.DossierFolders = MapDossierFoldersRecursive(childDossiers);
                }
            }

            return dossierFoldersModel;
        }        

        #endregion

        #endregion
    }
}