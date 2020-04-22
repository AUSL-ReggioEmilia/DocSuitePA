using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Entities;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows
{
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class FascicleService
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webApiClient;
        private static IEnumerable<LogCategory> _logCategories;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(FascicleService));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public FascicleService(ILogger logger, IWebAPIClient webApiClient)
        {
            _logger = logger;
            _webApiClient = webApiClient;
        }
        #endregion

        #region [ Methods ]
        public async Task<Fascicle> GetByUDSIdAsync(Guid udsId)
        {
            return (await _webApiClient.GetFascicleDocumentUnitAsync(string.Format("$filter=IdUDS eq {0}&$expand=Fascicle", udsId))).FirstOrDefault()?.Fascicle;
        }
        public async Task<FascicleDocumentUnit> GetFascicleDocumentUnitAsync(Guid documentUnitId, Fascicle fascicle)
        {
            return (await _webApiClient.GetFascicleDocumentUnitAsync(string.Format("$filter=DocumentUnit/Id eq {0} and Fascicle/UniqueId eq {1}", documentUnitId, fascicle.UniqueId))).FirstOrDefault();
        }

        public Fascicle CreateFascicle(Category category, Contact manager, WorkflowMetadata metadata)
        {
            Fascicle entity = new Fascicle()
            {
                Conservation = 0,
                FascicleType = FascicleType.Procedure,
                Name = string.Format("{0} - {1} - {2}", metadata.CompanyCode, metadata.JobCode, metadata.SourceName),
                FascicleObject = metadata.DocumentDescription,
                Category = category
            };

            if (manager != null)
            {
                entity.Contacts.Add(manager);
            }

            entity = _webApiClient.PostAsync(entity).Result;
            return entity;
        }

        public FascicleDocumentUnit CreateFascicleDocumentUnit(Guid documentUnitId, Fascicle fascicle)
        {
            FascicleDocumentUnit fascicleDocumentUnit = GetFascicleDocumentUnitAsync(documentUnitId, fascicle).Result;
            if (fascicleDocumentUnit != null)
            {
                _logger.WriteInfo(new LogMessage(string.Format("CreateFascicleDocumentUnit -> DocumentUnitId {0} already exist in fascicle Id {1}", documentUnitId, fascicle.UniqueId)), LogCategories);
                return fascicleDocumentUnit;
            }
            fascicleDocumentUnit = new FascicleDocumentUnit
            {
                DocumentUnit = new DocumentUnit(documentUnitId),
                Fascicle = fascicle
            };

            fascicleDocumentUnit = _webApiClient.PostAsync(fascicleDocumentUnit).Result;
            //todo: insert Folder
            return fascicleDocumentUnit;
        }
        #endregion
    }
}
