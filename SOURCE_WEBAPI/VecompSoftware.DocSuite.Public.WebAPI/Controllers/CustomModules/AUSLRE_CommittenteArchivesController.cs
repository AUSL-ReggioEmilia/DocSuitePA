using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.Document;
using VecompSoftware.DocSuite.Public.Core.Models.Customs.AUSL_RE.BandiDiGara;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.UDS;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using BandiDiGaraModels = VecompSoftware.DocSuite.Public.Core.Models.Customs.AUSL_RE.BandiDiGara;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.CustomModules
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    [AllowAnonymous]
    public class AUSLRE_CommittenteArchivesController : ODataController
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly Guid _instanceId;
        private readonly IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        private static string _dataScadenza = "DataScadenza";
        private static string _dataPubblicazione = "DataPubblicazione";
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
        public AUSLRE_CommittenteArchivesController(IDataUnitOfWork unitOfWork, ILogger logger, IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService)
            : base()
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _documentService = documentService;
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

                int count = RetrieveCountActiveStatusForCommittente(finder);

                return Ok(count);
            }, _logger, _logCategories);
        }

        [HttpPost]
        public IHttpActionResult SearchArchiveByGrid(ODataActionParameters parameter)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ArchiveFinderModel finder = parameter[ODataConfig.ODATA_FINDER_PARAMETER] as ArchiveFinderModel;
                List<AUSLRE_BandiModel_TableValue> resultModel = new List<AUSLRE_BandiModel_TableValue>();


                resultModel = RetrieveResultsFilteredForCommittente(finder);

                List<BandiDiGaraModels.ArchiveModel> archives = new List<BandiDiGaraModels.ArchiveModel>();
                foreach (AUSLRE_BandiModel_TableValue model in resultModel)
                {
                    archives.Add(new BandiDiGaraModels.ArchiveModel()
                    {
                        UniqueId = model.UDSId,
                        Subject = model.Subject,
                        Metadatas = new List<MetadataModel>()
                        {
                            new MetadataModel("Data pubblicazione", $"{model.DataPubblicazione: dd/MM/yyyy}", null, string.Empty, string.Empty),
                            new MetadataModel("Data scadenza", $"{model.DataScadenza: dd/MM/yyyy}", null, string.Empty, string.Empty),
                            new MetadataModel("Categoria", model.Categoria, null, string.Empty, string.Empty)}
                    });
                }

                return Ok(archives);
            }, _logger, _logCategories);
        }

        [HttpGet]
        public async Task<BandiDiGaraModels.ArchiveModel> GetArchiveInfo(Guid uniqueId)
        {
            return await CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                ICollection<AUSLRE_BandiModel_TableValue> resultModels = _unitOfWork.Repository<UDSFieldList>().ExecuteModelFunction<AUSLRE_BandiModel_TableValue>(CommonDefinition.SQL_FX_Get_UDS_T_Committente_GetArchiveInfo,
                 new QueryParameter(CommonDefinition.SQL_Param_UDSFieldList_IdUDS, uniqueId));

                AUSLRE_BandiModel_TableValue tableValue = resultModels.FirstOrDefault();
                List<ModelDocument.Document> documents = new List<ModelDocument.Document>();
                BandiDiGaraModels.ArchiveModel result = new BandiDiGaraModels.ArchiveModel()
                {
                    UniqueId = tableValue.UDSId,
                    Subject = tableValue.Subject,
                    Metadatas = new List<MetadataModel>()
                        {
                            new MetadataModel(tableValue.DataScadenzaLabel, $"{tableValue.DataScadenza: dd/MM/yyyy}", null, string.Empty, string.Empty),
                            new MetadataModel(tableValue.DataPubblicazioneLabel, $"{tableValue.DataPubblicazione: dd/MM/yyyy}", null, string.Empty, string.Empty),
                            new MetadataModel("Categoria", tableValue.Categoria, null, string.Empty, string.Empty),
                            new MetadataModel("Descrizione", tableValue.Descrizione, null, string.Empty, string.Empty),
                            new MetadataModel("Informazioni", string.IsNullOrEmpty(tableValue.Informazioni)?string.Empty:tableValue.Informazioni, null, string.Empty, string.Empty),
                            new MetadataModel("Istruttore pratica", string.IsNullOrEmpty(tableValue.IstruttorePratica)?string.Empty:tableValue.IstruttorePratica, null, string.Empty, string.Empty),
                            new MetadataModel("Provincia sede di gara", string.IsNullOrEmpty(tableValue.ProvinciaSedeDiGara)?string.Empty:tableValue.ProvinciaSedeDiGara, null, string.Empty, string.Empty),
                            new MetadataModel("Indirizzo sede di gara", string.IsNullOrEmpty(tableValue.IndirizzoSedeDiGara)?string.Empty:tableValue.IndirizzoSedeDiGara, null, string.Empty, string.Empty),
                            new MetadataModel("Codice CPV", string.IsNullOrEmpty(tableValue.CodiceCpv)?string.Empty:tableValue.CodiceCpv, null, string.Empty, string.Empty),
                            new MetadataModel("Codice CIG", string.IsNullOrEmpty(tableValue.CodiceCig)?string.Empty:tableValue.CodiceCig, null, string.Empty, string.Empty),
                            new MetadataModel("Amministrazione aggiudicatrice", string.IsNullOrEmpty(tableValue.AmministrazioneAggiudicatrice)?string.Empty:tableValue.AmministrazioneAggiudicatrice, null, string.Empty, string.Empty),
                            new MetadataModel("Amministrazione", string.IsNullOrEmpty(tableValue.Amministrazione)?string.Empty:tableValue.Amministrazione, null, string.Empty, string.Empty),
                            new MetadataModel("Responsabile di procedimento", string.IsNullOrEmpty(tableValue.ResponsabileProcedimento)?string.Empty:tableValue.ResponsabileProcedimento, null, string.Empty, string.Empty),
                            new MetadataModel("URL pubblicazione", string.IsNullOrEmpty(tableValue.UrlPubblicazione)?string.Empty:tableValue.UrlPubblicazione, null, string.Empty, string.Empty),
                            new MetadataModel("Link esterno", string.IsNullOrEmpty(tableValue.LinkEsterno)?string.Empty:tableValue.LinkEsterno, null, string.Empty, string.Empty),
                            new MetadataModel("Senza importo", !tableValue.SenzaImporto.HasValue?string.Empty:tableValue.SenzaImporto.Value?"Si":"No", null, string.Empty, string.Empty)
                        }
                };
                foreach (Guid idDocument in resultModels.Where(x => x.IdDocument.HasValue).Select(x => x.IdDocument.Value))
                {
                    documents.AddRange(await _documentService.GetDocumentsFromChainAsync(idDocument));
                }
                result.Documents = documents.OrderBy(f => f.CreatedDate.Value).Select(f => new Core.Models.Domains.Commons.DocumentModel(f.IdDocument, f.Name)).ToList();
                return result;
            }, _logger, _logCategories);
        }


        public int RetrieveCountActiveStatusForCommittente(ArchiveFinderModel finder)
        {
            return _unitOfWork.Repository<UDSFieldList>().ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Get_UDS_T_Committente_CountActiveStatus,
                   new QueryParameter(CommonDefinition.SQL_Param_UDS_SelectedMenuUniqueId, finder.ChildMenuUniqueId == Guid.Empty ? finder.ParentMenuUniqueId : finder.ChildMenuUniqueId),
                   new QueryParameter(CommonDefinition.SQL_Param_UDS_Subject, string.IsNullOrEmpty(finder.Subject) ? string.Empty : finder.Subject),
                   new QueryParameter(CommonDefinition.SQL_Param_UDS_Year, (short)finder.Year),
                   new QueryParameter(CommonDefinition.SQL_Param_UDS_IsIntranet, finder.IsIntranet));
        }

        public List<AUSLRE_BandiModel_TableValue> RetrieveResultsFilteredForCommittente(ArchiveFinderModel finder)
        {
            return _unitOfWork.Repository<UDSFieldList>().ExecuteModelFunction<AUSLRE_BandiModel_TableValue>(
                    finder.OrderColumn == _dataScadenza && !finder.OrderByDesc ?
                    CommonDefinition.SQL_FX_Get_UDS_T_Committente_FilterByDataScadenzaASC : finder.OrderColumn == _dataScadenza && finder.OrderByDesc ?
                    CommonDefinition.SQL_FX_Get_UDS_T_Committente_FilterByDataScadenzaDESC : finder.OrderColumn == _dataPubblicazione && !finder.OrderByDesc ?
                    CommonDefinition.SQL_FX_Get_UDS_T_Committente_FilterByDataPubblicazioneDESC : CommonDefinition.SQL_FX_Get_UDS_T_Committente_FilterByDataPubblicazioneASC,
                   new QueryParameter(CommonDefinition.SQL_Param_UDS_SelectedMenuUniqueId, finder.ChildMenuUniqueId == Guid.Empty ? finder.ParentMenuUniqueId : finder.ChildMenuUniqueId),
                   new QueryParameter(CommonDefinition.SQL_Param_UDS_Subject, string.IsNullOrEmpty(finder.Subject) ? string.Empty : finder.Subject),
                   new QueryParameter(CommonDefinition.SQL_Param_UDS_Year, (short)finder.Year),
                   new QueryParameter(CommonDefinition.SQL_Param_UDS_IsIntranet, finder.IsIntranet),
                   new QueryParameter(CommonDefinition.SQL_Param_UDS_Skip, finder.Skip),
                   new QueryParameter(CommonDefinition.SQL_Param_UDS_Top, finder.Top)).ToList();
        }
        #endregion
    }
}