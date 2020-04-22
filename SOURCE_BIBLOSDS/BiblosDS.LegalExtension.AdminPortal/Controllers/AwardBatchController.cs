using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.AwardBatches;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Services.AwardBatches.Interactors;
using BiblosDS.LegalExtension.AdminPortal.Helpers;
using BiblosDS.LegalExtension.AdminPortal.Infrastructure.Services.Common;
using BiblosDS.LegalExtension.AdminPortal.Models;
using BiblosDS.LegalExtension.AdminPortal.ServiceReferenceDocument;
using BiblosDS.LegalExtension.AdminPortal.ViewModel;
using BiblosDS.LegalExtension.AdminPortal.ViewModel.AwardBatches;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.Services;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Utility;
using Kendo.Mvc.UI;
using log4net;
using Newtonsoft.Json;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BiblosDS.LegalExtension.AdminPortal.Controllers
{
    [Authorize()]
    public class AwardBatchController : Controller
    {
        #region [ Fields ]
        private readonly ILog _logger = LogManager.GetLogger(typeof(AwardBatchController));
        private readonly ILogger _loggerService;
        private readonly PreservationService _preservationService;
        private readonly SavePDVXmlInteractor _savePDVXmlInteractor;
        private readonly SaveRDVXmlInteractor _saveRDVXmlInteractor;
        private readonly UploadHelper _uploadHelper;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public AwardBatchController()
        {
            _loggerService = new LoggerService(_logger);
            _preservationService = new PreservationService();
            _savePDVXmlInteractor = new SavePDVXmlInteractor(_loggerService);
            _saveRDVXmlInteractor = new SaveRDVXmlInteractor(_loggerService);
            _uploadHelper = new UploadHelper();
        }
        #endregion

        #region [ Methods ]
        [NoCache]
        [Authorize]
        public ActionResult Index(Guid id, Guid? idPreservation)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                DocumentArchive archive = ArchiveService.GetArchive(id);
                if (archive == null)
                {
                    throw new Exception(string.Format("Nessun archivio trovato con id {0}", id));
                }

                AwardBatchViewModel model = new AwardBatchViewModel()
                {
                    ArchiveName = archive.Name,
                    IdArchive = archive.IdArchive,
                    IdPreservation = idPreservation
                };

                if (idPreservation.HasValue && idPreservation.Value != Guid.Empty)
                {
                    model.FromDate = null;
                    model.ToDate = null;
                }
                return View(model);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult GetAwardBatches(Guid id, Guid? idPreservation, DateTime? fromDate, DateTime? toDate, [DataSourceRequest] DataSourceRequest request)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                IList<AwardBatch> batches = _preservationService.GetAwardBatches(id, idPreservation, fromDate, toDate?.AddDays(1), request.PageSize, (request.Page - 1) * request.PageSize, out int total).ToList();
                DataSourceResult result = new DataSourceResult()
                {
                    Total = total,
                    Data = batches
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }, _loggerService);
        }


        [NoCache]
        [Authorize]
        public ActionResult Detail(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                AwardBatch awardBatch = _preservationService.GetAwardBatch(id);
                if (awardBatch == null)
                {
                    throw new Exception(string.Format("Nessun pacchetto di versamento trovato con id {0}", id));
                }

                AwardBatchDetailsViewModel model = new AwardBatchDetailsViewModel()
                {
                    IdArchive = awardBatch.IdArchive,
                    IdAwardBatch = awardBatch.IdAwardBatch,
                    Name = awardBatch.Name,
                    IsOpen = awardBatch.IsOpen
                };
                return View(model);
            }, _loggerService);
        }


        [NoCache]
        [Authorize]
        public ActionResult GetAwardBatchDocuments(Guid id, [DataSourceRequest] DataSourceRequest request)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                var docs = _preservationService.GetAwardBatchDocuments(id, (request.Page - 1) * request.PageSize, request.PageSize, out int total)
                  .Select(p => new DocumentItem()
                  {
                      IdDocument = p.IdDocument,
                      Name = p.Name,
                      DateMain = p.DateMain,
                      DocumentHash = UtilityService.ToHexString(Convert.FromBase64String(p.DocumentHash))
                  }).ToList();

                DataSourceResult result = new DataSourceResult()
                {
                    Total = total,
                    Data = docs
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult ViewAwardPDV(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                AwardBatch awardBatch = _preservationService.GetAwardBatch(id);
                if (awardBatch.IdPDVDocument.HasValue)
                {
                    using (DocumentsClient client = new DocumentsClient())
                    {
                        DocumentContent content = client.GetDocumentContentById(awardBatch.IdPDVDocument.Value);
                        if (content != null)
                        {
                            return View(new WindowFileContentViewModel() { Content = System.Text.Encoding.UTF8.GetString(content.Blob) });
                        }
                        _loggerService.Warn(string.Format("ViewAwardPDV -> nessun documento trovato con Id {0}. Si procede con una nuova generazione del pacchetto di versamento.", awardBatch.IdPDVDocument));
                    }
                }

                string pdv = _preservationService.CreateAwardBatchPDVXml(awardBatch);
                SavePDVXml(pdv, awardBatch);
                WindowFileContentViewModel model = new WindowFileContentViewModel()
                {
                    Content = pdv
                };
                return View(model);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult ViewAwardRDV(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                AwardBatch awardBatch = _preservationService.GetAwardBatch(id);
                if (awardBatch.IdRDVDocument.HasValue)
                {
                    using (DocumentsClient client = new DocumentsClient())
                    {
                        DocumentContent content = client.GetDocumentContentById(awardBatch.IdRDVDocument.Value);
                        if (content != null)
                        {
                            return View(new WindowFileContentViewModel() { Content = System.Text.Encoding.UTF8.GetString(content.Blob) });
                        }                        
                    }
                    _loggerService.Warn(string.Format("ViewAwardRDV -> nessun documento trovato con Id {0}.", awardBatch.IdRDVDocument));
                }

                WindowFileContentViewModel model = new WindowFileContentViewModel()
                {
                    Content = string.Empty
                };
                return View(model);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        [HttpPost]
        public ActionResult CloseAwardBatch(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                AwardBatch awardBatch = _preservationService.GetAwardBatch(id);
                if (awardBatch.IsOpen)
                {
                    _preservationService.CloseAwardBatch(awardBatch);
                    string xml = string.Empty;
                    if (awardBatch.IdPDVDocument.HasValue)
                    {
                        using (DocumentsClient client = new DocumentsClient())
                        {
                            DocumentContent content = client.GetDocumentContentById(awardBatch.IdPDVDocument.Value);
                            if (content != null)
                            {
                                xml = Encoding.UTF8.GetString(content.Blob);
                            }
                            _loggerService.Warn(string.Format("CloseAwardBatch -> nessun documento trovato con Id {0}. Si procede con una nuova generazione del pacchetto di versamento.", awardBatch.IdPDVDocument));
                        }
                    }

                    if (string.IsNullOrEmpty(xml))
                    {
                        xml = _preservationService.CreateAwardBatchPDVXml(awardBatch);
                        SavePDVXml(xml, awardBatch);
                    }

                    DocumentArchive archive = ArchiveService.GetArchive(awardBatch.IdArchive);
                    if (!archive.Name.Equals(ConfigurationHelper.RDVArchiveName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        SaveRDVXml(xml, awardBatch);
                    }                    
                }
                return Content(string.Empty);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult AwardBatchRDVSign(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                DocumentArchive archive = ArchiveService.GetArchive(id);
                ViewBag.messageId = Guid.NewGuid().ToString();
                AwardBatchRDVSignViewModel viewModel = new AwardBatchRDVSignViewModel()
                {
                    ArchiveName = archive.Name,
                    IdArchive = archive.IdArchive
                };
                return View(viewModel);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult GetRDVToSign([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                DataSourceResult result = new DataSourceResult();
                ICollection<AwardBatch> toSignBatches = _preservationService.GetToSignAwardBatchRDV(id);
                result.Total = toSignBatches.Count;
                result.Data = toSignBatches;
                return Json(result, JsonRequestBehavior.AllowGet);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        [HttpPost]
        public ActionResult DownloadRDVToSign(string data)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                string zipFile = string.Empty;
                try
                {
                    ICollection<Guid> ids = JsonConvert.DeserializeObject<ICollection<Guid>>(HttpUtility.UrlDecode(data));
                    zipFile = CreateRDVToSignZipToDownload(ids);
                    return File(System.IO.File.ReadAllBytes(zipFile), System.Net.Mime.MediaTypeNames.Application.Zip);
                }
                finally
                {
                    if (System.IO.File.Exists(zipFile))
                    {
                        System.IO.File.Delete(zipFile);
                    }
                }
            }, _loggerService);
        }

        [NonAction]
        private string CreateRDVToSignZipToDownload(ICollection<Guid> awardBatchIds)
        {
            string destination = Path.Combine(ConfigurationHelper.GetAppDataPath(), string.Format("RDVToSign_{0:yyyyMMddHHmmss}.zip", DateTime.Now));
            string folderDestination = Path.Combine(ConfigurationHelper.GetAppDataPath(), Guid.NewGuid().ToString());
            try
            {
                Directory.CreateDirectory(folderDestination);
                using (IWritableArchive archive = ArchiveFactory.Create(ArchiveType.Zip))
                using (DocumentsClient client = new DocumentsClient())
                {
                    Parallel.ForEach(awardBatchIds, (idAwardBatch) =>
                    {
                        AwardBatch awardBatch = _preservationService.GetAwardBatch(idAwardBatch);
                        if (!awardBatch.IdRDVDocument.HasValue)
                        {
                            _logger.WarnFormat("CreateRDVToSignZipToDownload -> RDV document not found for award batch {0}", idAwardBatch);
                            return;
                        }

                        DocumentContent content = client.GetDocumentContentById(awardBatch.IdRDVDocument.Value);
                        System.IO.File.WriteAllBytes(Path.Combine(folderDestination, string.Concat(idAwardBatch, "_", UtilityService.GetSafeFileName(content.Description))), content.Blob);
                    });
                    archive.AddAllFromDirectory(folderDestination);
                    archive.SaveTo(destination, CompressionType.Deflate);
                }
            }
            finally
            {
                if (Directory.Exists(folderDestination))
                {
                    Directory.Delete(folderDestination, true);
                }
            }
            return destination;
        }

        [NoCache]
        [Authorize]
        [HttpPost]
        public ActionResult RDVUploadSignedFile(IEnumerable<HttpPostedFileBase> files, string metaData, string messageId, Guid idArchive)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                if (string.IsNullOrEmpty(metaData))
                {
                    _uploadHelper.UploadSave(files);
                    return Content(string.Empty);
                }

                Models.FileResult fileBlob = _uploadHelper.ChunkUploadSave(files, metaData, messageId);
                if (fileBlob.Uploaded)
                {
                    fileBlob.ExtraDescription = JsonConvert.SerializeObject(UnZipRDVSignedFile(fileBlob.FileName, idArchive));
                    _uploadHelper.RemoveUploadedFile(fileBlob.FileName, messageId);
                }
                return Content(JsonConvert.SerializeObject(fileBlob), System.Net.Mime.MediaTypeNames.Text.Plain);
            }, _loggerService);
        }

        [NonAction]
        private ICollection<UnZipReportViewModel> UnZipRDVSignedFile(string zipPath, Guid idArchive)
        {
            ICollection<UnZipReportViewModel> viewModel = new List<UnZipReportViewModel>();
            if (System.IO.File.Exists(zipPath))
            {
                using (Stream stream = System.IO.File.OpenRead(zipPath))
                using (IReader reader = ReaderFactory.Open(stream))
                using (DocumentsClient client = new DocumentsClient())
                {
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            string fileName = Path.GetFileName(reader.Entry.Key);
                            _logger.InfoFormat("UnZipRDVSignedFile -> Lettura file {0}", fileName);

                            string awardBatchIdStr = fileName.Split('_').FirstOrDefault();
                            if (Guid.TryParse(awardBatchIdStr, out Guid idAwardBatch))
                            {
                                if (!Path.GetExtension(fileName).Equals(".p7m", StringComparison.InvariantCultureIgnoreCase) && !Path.GetExtension(fileName).Equals(".tsd", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    _logger.WarnFormat("UnZipRDVSignedFile -> Il file {0} non risulta firmato", fileName);
                                    viewModel.Add(new UnZipReportViewModel()
                                    {
                                        ReferenceId = idAwardBatch,
                                        Description = string.Format("Il file <b>{0}</b> non risulta firmato ed è stato scartato.", fileName),
                                        LogType = UnZipReportViewModel.TYPE_WARN
                                    });
                                    continue;
                                }

                                AwardBatch awardBatch = _preservationService.GetAwardBatch(idAwardBatch);
                                if (awardBatch == null)
                                {
                                    _logger.WarnFormat("UnZipRDVSignedFile -> Nessun pacchetto di versamento trovato con id {0}", idAwardBatch);
                                    viewModel.Add(new UnZipReportViewModel()
                                    {
                                        ReferenceId = idAwardBatch,
                                        Description = string.Format("Nessun pacchetto di versamento trovato con id {0}", idAwardBatch),
                                        LogType = UnZipReportViewModel.TYPE_ERROR
                                    });
                                    continue;
                                }

                                if (awardBatch.IdArchive != idArchive)
                                {
                                    _logger.WarnFormat("UnZipRDVSignedFile -> Il pacchetto di versamento {0} non fa parte dell'archivio {0}", idAwardBatch, idArchive);
                                    viewModel.Add(new UnZipReportViewModel()
                                    {
                                        ReferenceId = idAwardBatch,
                                        Description = string.Format("Il pacchetto di versamento <b>{0}</b> non fa parte dell'archivio selezionato e verrà scartato.", awardBatch.Name),
                                        LogType = UnZipReportViewModel.TYPE_ERROR
                                    });
                                    continue;
                                }

                                if (awardBatch.IsRDVSigned.HasValue && awardBatch.IsRDVSigned.Value)
                                {
                                    _logger.WarnFormat("UnZipRDVSignedFile -> Il pacchetto di versamento con id {0} risulta già firmato.", idAwardBatch);
                                    viewModel.Add(new UnZipReportViewModel()
                                    {
                                        ReferenceId = idAwardBatch,
                                        Description = string.Format("Il pacchetto di versamento <b>{0}</b> risulta già firmato e verrà scartato.", awardBatch.Name),
                                        LogType = UnZipReportViewModel.TYPE_WARN
                                    });
                                    continue;
                                }

                                if (!awardBatch.IdRDVDocument.HasValue)
                                {
                                    _logger.WarnFormat("UnZipRDVSignedFile -> Nessun RDV presente per il pacchetto di versamento trovato con id {0}", idAwardBatch);
                                    viewModel.Add(new UnZipReportViewModel()
                                    {
                                        ReferenceId = idAwardBatch,
                                        Description = string.Format("Nessun RDV presente per il pacchetto di versamento <b>{0}</b>", awardBatch.Name),
                                        LogType = UnZipReportViewModel.TYPE_ERROR
                                    });
                                    continue;
                                }                                

                                _logger.DebugFormat("UnZipRDVSignedFile -> CheckOut documento con id {0}", awardBatch.IdRDVDocument);
                                Document document = client.CheckOutDocument(awardBatch.IdRDVDocument.Value, idAwardBatch.ToString(), Library.Common.Enums.DocumentContentFormat.Binary, false);
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    reader.WriteEntryTo(ms);
                                    document.Content = new DocumentContent(ms.ToArray());
                                    document.Name = UtilityService.GetSafeFileName(fileName.Substring(fileName.IndexOf('_') + 1));
                                    DocumentAttributeValue fileNameAttribute = document.AttributeValues.FirstOrDefault(x => x.Attribute.Name.Equals("Filename", StringComparison.InvariantCultureIgnoreCase));
                                    fileNameAttribute.Value = document.Name;
                                    _logger.DebugFormat("UnZipRDVSignedFile -> CheckIn documento firmato con id {0}", awardBatch.IdRDVDocument);
                                    client.CheckInDocument(document, idAwardBatch.ToString(), Library.Common.Enums.DocumentContentFormat.Binary, null);
                                }

                                awardBatch.IsRDVSigned = true;
                                _preservationService.UpdateAwardBatch(awardBatch);
                                viewModel.Add(new UnZipReportViewModel()
                                {
                                    ReferenceId = idAwardBatch,
                                    Description = string.Format("Pacchetto di versamento <b>{0}</b> aggiornato correttamente", awardBatch.Name),
                                    LogType = UnZipReportViewModel.TYPE_SUCCESS
                                });
                            }
                        }
                    }
                }
            }
            return viewModel;
        }

        [NoCache]
        [Authorize]
        [HttpPost]
        public ActionResult RDVRemoveUploadSignedFile(string[] fileNames, string messageId)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                if (fileNames != null)
                {
                    foreach (string file in fileNames)
                    {
                        string fileName = Path.GetFileName(file);
                        _uploadHelper.RemoveUploadedFile(fileName, messageId);
                    }
                }
                return Content(string.Empty);
            }, _loggerService);
        }

        private void SavePDVXml(string xml, AwardBatch awardBatch)
        {
            SaveAwardBatchXMLRequestModel pdvRequestModel = new SaveAwardBatchXMLRequestModel()
            {
                IdAwardBatch = awardBatch.IdAwardBatch,
                ArchiveName = ConfigurationHelper.PDVArchiveName,
                Content = Encoding.UTF8.GetBytes(xml)
            };
            _savePDVXmlInteractor.Process(pdvRequestModel);
        }

        private void SaveRDVXml(string xml, AwardBatch awardBatch)
        {
            SaveAwardBatchXMLRequestModel rdvRequestModel = new SaveAwardBatchXMLRequestModel()
            {
                IdAwardBatch = awardBatch.IdAwardBatch,
                ArchiveName = ConfigurationHelper.RDVArchiveName,
                Content = Encoding.UTF8.GetBytes(xml)
            };
            _saveRDVXmlInteractor.Process(rdvRequestModel);
        }
        #endregion
    }
}
