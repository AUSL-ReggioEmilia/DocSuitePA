using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiblosDS.LegalExtension.AdminPortal.Helpers;
using Kendo.Mvc.UI;
using System.Data.SqlClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;
using BiblosDS.LegalExtension.AdminPortal.Models;
using log4net;
using BiblosDS.Library.Common.Services;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Objects;
using BiblosDS.LegalExtension.AdminPortal.ViewModel;
using BiblosDS.Library.Common.Preservation.Services;
using System.IO;
using BiblosDS.LegalExtension.AdminPortal.ServiceReferenceDocument;
using SharpCompress.Archives;
using SharpCompress.Common;
using Newtonsoft.Json;
using BiblosDS.LegalExtension.AdminPortal.ViewModel.Home;
using BiblosDS.LegalExtension.AdminPortal.Infrastructure.Services.Common;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Services.Documents.Interactors;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Documents;
using System.Threading.Tasks;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Services.AwardBatches.Interactors;

namespace BiblosDS.LegalExtension.AdminPortal.Controllers
{
    public partial class HomeController : Controller
    {
        #region [ Fields ]
        private readonly ILog _logger = LogManager.GetLogger(typeof(HomeController));
        private readonly PreservationService _preservationService;
        private readonly ILogger _loggerService;
        private readonly SearchDocumentsInteractor _searchDocumentsInteractor;
        private readonly SavePDVXmlInteractor _savePDVXmlInteractor;
        private readonly SaveRDVXmlInteractor _saveRDVXmlInteractor;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public HomeController()
        {
            _preservationService = new PreservationService();
            _loggerService = new LoggerService(_logger);
            _searchDocumentsInteractor = new SearchDocumentsInteractor(_loggerService);
            _savePDVXmlInteractor = new SavePDVXmlInteractor(_loggerService);
            _saveRDVXmlInteractor = new SaveRDVXmlInteractor(_loggerService);
        }
        #endregion

        #region [ Methods ]        
        [NoCache]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ErrorLog()
        {
            return View();
        }


        [NoCache]
        public ActionResult ReadLogs(string table, [DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();
            List<LogModel> data = new List<LogModel>();
            try
            {
                string connectionString;
                try
                {
                    connectionString = RoleEnvironment.GetConfigurationSettingValue("SqlLogConnectionString");
                }
                catch (Exception)
                {
                    connectionString = ConfigurationManager.AppSettings["SqlLogConnectionString"];
                }
                long total = 0;
                int page = request.Page - 1;
                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    string sql = string.Format(@"
Select [PartitionKey]
      ,[RowKey]
      ,[RoleInstance]
      ,[DeploymentId]
      ,[Timestamp]
      ,[Message]
      ,[Level]
      ,[LoggerName]
      ,[Domain]
      ,[ThreadName]
      ,[Identity]
From
(SELECT ROW_NUMBER() OVER(ORDER BY Timestamp DESC) AS RowNum, [PartitionKey]
      ,[RowKey]
      ,[RoleInstance]
      ,[DeploymentId]
      ,[Timestamp]
      ,[Message]
      ,[Level]
      ,[LoggerName]
      ,[Domain]
      ,[ThreadName]
      ,[Identity]
From {2} {3}) as sub
Where sub.RowNum between {0} and {1}", page * request.PageSize, (page * request.PageSize) + request.PageSize, table, parseWhere(request.Filters));
                    cnn.Open();
                    using (SqlCommand cmd = new SqlCommand(string.Format("select count(*) from {0}", table), cnn))
                    {
                        total = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand(sql, cnn))
                    {

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                data.Add(new LogModel
                                {
                                    RowKey = dr.GetString(1),
                                    RoleInstance = dr.GetString(2),
                                    DeploymentId = dr.GetString(3),
                                    Timestamp = dr.GetDateTime(4),
                                    Message = dr.GetString(5),
                                    Level = dr.GetString(6),
                                    LoggerName = dr.GetString(7),
                                    Domain = dr.GetString(8),
                                    ThreadName = dr.GetString(9),
                                    Identity = dr.GetString(10),
                                });
                            }
                        }
                    }

                }
                result.Total = (int)total;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Errors = ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string parseWhere(IList<Kendo.Mvc.IFilterDescriptor> iList)
        {
            if (iList == null)
                return "";
            foreach (Kendo.Mvc.IFilterDescriptor item in iList)
            {

            }
            return "";
        }

        [NoCache]
        [Authorize]
        public ActionResult DistributionPackages()
        {
            return View();
        }

        [NoCache]
        [Authorize]
        public ActionResult GetArchives(string text)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                ICollection<PreservationArchiveInfoResponse> archives = ArchiveService.GetLegalArchives(string.Empty);
                if (!string.IsNullOrEmpty(text))
                {
                    archives = archives.Where(x => x.Archive.Name.ToLower().Contains(text.ToLower())).ToList();
                }
                return Json(archives.Select(s => s.Archive), JsonRequestBehavior.AllowGet);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult DistributionPackagesGrid(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                DistributionPackagesSearchGridViewModel viewModel = new DistributionPackagesSearchGridViewModel()
                {
                    IdArchive = id,
                    ArchiveAttributes = AttributeService.GetAttributesFromArchive(id).OrderBy(x => x.Name).Select(s => new AttributeModel()
                    {
                        Name = s.Name,
                        Description = string.IsNullOrEmpty(s.Description) ? s.Name : s.Description
                    }).ToList()
                };

                return PartialView("_DistributionPackagesGrid", viewModel);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        [HttpGet]
        public ActionResult DynamicFormFields(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                ICollection<DocumentAttribute> archiveAttributes = AttributeService.GetAttributesFromArchive(id);
                DynamicFormFieldsViewModel viewModel = new DynamicFormFieldsViewModel()
                {
                    DynamicControlViewModels = archiveAttributes.OrderBy(x => x.Name).Select(s => new DynamicControlViewModel()
                    {
                        Caption = string.IsNullOrEmpty(s.Description) ? s.Name : s.Description,
                        ControlName = s.Name,
                        ControlType = s.AttributeType
                    }).ToList()
                };
                return PartialView("_DynamicFormFields", viewModel);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        [HttpPost]
        public ActionResult ExportFullPDD(Guid? idArchive, DateTime? fromDate, DateTime? toDate, string dynamicFilters)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                if (!idArchive.HasValue)
                {
                    _logger.InfoFormat("ExportFullPDD -> nessun id archivio passato per la ricerca");
                    return Content(string.Empty);
                }

                IDictionary<string, string> filters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(dynamicFilters))
                {
                    filters = JsonConvert.DeserializeObject<IDictionary<string, string>>(dynamicFilters);
                }

                SearchDocumentsRequestModel requestModel = new SearchDocumentsRequestModel()
                {
                    IdArchive = idArchive.Value,
                    FromDate = fromDate,
                    ToDate = toDate,
                    DynamicFilters = filters
                };
                SearchDocumentsResponseModel responseModel = _searchDocumentsInteractor.Process(requestModel);
                return ExportPDD(JsonConvert.SerializeObject(responseModel.Documents.Select(s => s.IdDocument)));
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        [HttpPost]
        public ActionResult ExportPDD(string data)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                ICollection<Guid> ids = JsonConvert.DeserializeObject<ICollection<Guid>>(HttpUtility.UrlDecode(data));
                Guid processId = Guid.NewGuid();
                string path = Path.Combine(ConfigurationHelper.GetAppDataPath(), processId.ToString());
                Directory.CreateDirectory(path);
                try
                {
                    foreach (Guid id in ids)
                    {
                        Document document = DocumentService.GetDocument(id);
                        using (DocumentsClient client = new DocumentsClient())
                        {
                            document.Content = client.GetDocumentContentById(id);
                        }

                        //Copied from preservation logic
                        document.AttributeValues = AttributeService.GetAttributeValues(document.IdDocument);
                        var fileName = string.Concat(_preservationService.PreservationDocumentFileName(document), (Path.GetExtension(document.Name)));
                        System.IO.File.WriteAllBytes(Path.Combine(path, fileName), document.Content.Blob);

                        if (document.IdPreservation.HasValue)
                        {
                            Preservation preservation = _preservationService.GetPreservation(document.IdPreservation.Value, false);
                            string[] files = Directory.GetFiles(preservation.Path).Where(x => x.Contains("INDICE_") || x.Contains("CHIUSURA_") || x.Contains("IPDA_") || x.Contains("LOTTI_")).ToArray();
                            foreach (string file in files.Where(f => !System.IO.File.Exists(Path.Combine(path, Path.GetFileName(f)))))
                            {
                                System.IO.File.Copy(file, Path.Combine(path, Path.GetFileName(file)), true);
                            }
                        }
                    }
                    string zipPath = CreatePDDZip(path);
                    return File(System.IO.File.ReadAllBytes(zipPath), System.Net.Mime.MediaTypeNames.Application.Zip);
                }
                finally
                {
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                }
            }, _loggerService);
        }

        [NonAction]
        private string CreatePDDZip(string path)
        {
            string destination = Path.Combine(path, string.Format("pacchetto di distribuzione_{0:ddMMyyyyHHmmss}.zip", DateTime.Now));
            using (IWritableArchive archive = ArchiveFactory.Create(ArchiveType.Zip))
            {
                archive.AddAllFromDirectory(path);
                archive.SaveTo(destination, CompressionType.Deflate);
            }
            return destination;
        }
        #endregion
    }
}
