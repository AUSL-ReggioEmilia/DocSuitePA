using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using BandiDiGaraModels = VecompSoftware.DocSuite.Public.Core.Models.Customs.AUSL_RE.BandiDiGara;
using VecompSoftware.DocSuite.Public.Core.Models.Customs.AUSL_RE.BandiDiGara;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.CustomModules
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    [AllowAnonymous]
    public class AUSLRE_BandiDiGaraArchivesController : ODataController
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly Guid _instanceId;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(AUSLRE_BandiDiGaraArchivesController));
                }
                return _logCategories;
            }
        }

        protected string Username { get; }

        protected string Domain { get; }

        protected string SingleQuoteCode { get; }
        #endregion

        #region [ Constructor ]
        public AUSLRE_BandiDiGaraArchivesController(IDataUnitOfWork unitOfWork, ILogger logger)
            : base()
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _instanceId = Guid.NewGuid();
        }
        #endregion

        #region [ Methods ]
        [HttpPost]
        public IHttpActionResult CountArchiveByGrid(ODataActionParameters parameter)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ArchiveFinderModel finder = parameter[ODataConfig.ODATA_FINDER_PARAMETER] as ArchiveFinderModel;
                int count = 8;
                return Ok(count);
            }, _logger, _logCategories);
        }

        [HttpPost]
        public IHttpActionResult SearchArchiveByGrid(ODataActionParameters parameter)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ArchiveFinderModel finder = parameter[ODataConfig.ODATA_FINDER_PARAMETER] as ArchiveFinderModel;
                ICollection<BandiDiGaraModels.ArchiveModel> archives = new List<BandiDiGaraModels.ArchiveModel>
                {
                    new BandiDiGaraModels.ArchiveModel
                    {
                        ArchiveName = "Test Nome Archive 1",
                        Subject = "Test Oggetto Archive 1",
                        RegistrationDate = new DateTimeOffset(),
                    },
                    new BandiDiGaraModels.ArchiveModel
                    {
                        ArchiveName = "Test Nome Archive 2",
                        Subject = "Test Oggetto Archive 2",
                        RegistrationDate = new DateTimeOffset(),
                    },
                    new BandiDiGaraModels.ArchiveModel
                    {
                        ArchiveName = "Test Nome Archive 3",
                        Subject = "Test Oggetto Archive 3",
                        RegistrationDate = new DateTimeOffset(),
                    },
                    new BandiDiGaraModels.ArchiveModel
                    {
                        ArchiveName = "Test Nome Archive 4",
                        Subject = "Test Oggetto Archive 4",
                        RegistrationDate = new DateTimeOffset(),
                    }
                };

                return Ok(archives);
            }, _logger, _logCategories);
        }

        [HttpGet]
        public IHttpActionResult GetArchiveInfo(Guid uniqueId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                BandiDiGaraModels.ArchiveModel archive = new BandiDiGaraModels.ArchiveModel
                {
                    ArchiveName = "Test Nome Archive 1",
                    Subject = "Test Oggetto Archive 1",
                    RegistrationDate = new DateTimeOffset(),
                    Documents = new List<Core.Models.Domains.Commons.DocumentModel>
                    {
                        new Core.Models.Domains.Commons.DocumentModel(new Guid(), "doc1.pdf"),
                        new Core.Models.Domains.Commons.DocumentModel(new Guid(), "doc2.pdf")
                    },
                    Metadatas = new List<MetadataModel>
                    {
                        new MetadataModel("Metadato 1 Key", "Metadato 1 Value", new Guid()),
                        new MetadataModel("Metadato 2 Key", "Metadato 2 Value", new Guid())
                    }
                };

                return Ok(archive);
            }, _logger, _logCategories);
        }

        #endregion
    }
}