using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.Helpers;
using BiblosDS.LegalExtension.AdminPortal.Infrastructure.Services.Common;
using BiblosDS.LegalExtension.AdminPortal.ViewModel;
using BiblosDS.LegalExtension.AdminPortal.ViewModel.Ipda;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.IpdaDoc;
using BiblosDS.Library.Common.Preservation.Services;
using log4net;
using Newtonsoft.Json;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VecompSoftware.BiblosDS.Model.Parameters;
using VecompSoftware.Sign.Interfaces;
using VecompSoftware.Sign.Services;

namespace BiblosDS.LegalExtension.AdminPortal.Controllers
{
    [Authorize]
    public class IpdaController : Controller
    {
        #region [ Fields ]
        private readonly ILog _logger = LogManager.GetLogger(typeof(IpdaController));
        private readonly ILogger _loggerService;
        private readonly UploadHelper _uploadHelper;
        private readonly PreservationService _preservationService;
        private readonly ExtractionOptions _extractionOptions;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public IpdaController()
        {
            _uploadHelper = new UploadHelper();
            _preservationService = new PreservationService();
            _extractionOptions = new ExtractionOptions();
            _extractionOptions.ExtractFullPath = true;
            _extractionOptions.Overwrite = true;
            _extractionOptions.PreserveFileTime = true;
            _loggerService = new LoggerService(_logger);
        }
        #endregion

        #region [ Methods ]
        [NoCache]
        [Authorize]
        public ActionResult Index(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                Preservation preservation = _preservationService.GetPreservation(id, false);
                if (preservation == null)
                {
                    _loggerService.Warn(string.Format("Nessuna conservazione trovata con id {0}", id));
                    return HttpNotFound();
                }

                IpdaIndexViewModel model = new IpdaIndexViewModel() { Preservation = preservation };
                string preservationPath = preservation.Path;
                string closeFile = IpdaUtil.GetCloseFile(preservationPath);
                string ipdaXmlFile = IpdaUtil.GetIpdaXmlFile(preservationPath);
                string ipdaTsdFile = IpdaUtil.GetIpdaTsdFile(preservationPath);
                model.ToCreate = (!string.IsNullOrEmpty(closeFile) && string.IsNullOrEmpty(ipdaXmlFile) && string.IsNullOrEmpty(ipdaTsdFile));
                model.ToSign = !model.ToCreate && (!string.IsNullOrEmpty(ipdaXmlFile) && string.IsNullOrEmpty(ipdaTsdFile));
                model.ToClose = !model.ToCreate && !model.ToSign && (!string.IsNullOrEmpty(ipdaXmlFile) && !string.IsNullOrEmpty(ipdaTsdFile) && !preservation.CloseDate.HasValue);
                return View(model);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        [HttpPost]
        public ActionResult CreateIpda(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                try
                {
                    Preservation preservation = _preservationService.GetPreservation(id, false);
                    if (preservation == null)
                    {
                        _loggerService.Error(string.Format("Nessuna conservazione trovata con id {0}", id));
                        return Content("Errore: Conservazione non valida");
                    }

                    string closeFile = IpdaUtil.GetCloseFile(preservation.Path);
                    if (string.IsNullOrEmpty(closeFile))
                        return Content("Errore: Conservazione non valida");

                    IpdaConverter converter = new IpdaConverter();
                    Ipda ipda = converter.ConvertCloseFile(closeFile, "NomeFileInArchivio");
                    ipda.SaveAs(IpdaUtil.Close2IpdaFilename(closeFile));

                    return Content("Il file IPDA è stato generato con successo");
                }
                catch (Exception ex)
                {
                    _loggerService.Error(ex.Message, ex);
                    return Content(string.Concat("Errore durante la conversione del file di chiusura.<br/>", ex.Message));
                }
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult DownloadXml(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                Preservation preservation = _preservationService.GetPreservation(id, false);
                string xmlFile = IpdaUtil.GetIpdaXmlFile(preservation.Path);
                return File(xmlFile, System.Net.Mime.MediaTypeNames.Application.Zip, string.Concat(id, "_", Path.GetFileName(xmlFile)));
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        [HttpPost]
        public ActionResult UploadSignedFile(IEnumerable<HttpPostedFileBase> filesUpload)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                StringBuilder result = new StringBuilder();
                if (filesUpload != null)
                {
                    foreach (HttpPostedFileBase file in filesUpload)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string preservationIdStr = fileName.Split('_').FirstOrDefault();
                        if (Guid.TryParse(preservationIdStr, out Guid preservationId))
                        {
                            Preservation preservation = _preservationService.GetPreservation(preservationId, false);
                            if (preservation != null)
                            {
                                if (preservation.CloseDate.HasValue)
                                {
                                    string pattern = "Conservazione <b>{0}</b> del {1} <b>CHIUSA</b> il {2}.<br/>";
                                    result.AppendFormat(pattern, preservation.IdPreservation, preservation.StartDate, preservation.CloseDate);
                                    continue;
                                }
                                else
                                {
                                    string strNewFile = Path.Combine(preservation.Path, fileName.Split('_').LastOrDefault());
                                    if (System.IO.File.Exists(strNewFile))
                                        result.AppendFormat("File già presente!<br/>Il file verrà ignorato: {0}<br/>",
                                          Path.Combine(preservation.Path, preservation.Label, fileName.Split('_').LastOrDefault()));
                                    else
                                        file.SaveAs(strNewFile);
                                }
                            }
                        }
                        else
                        {
                            _loggerService.Warn(string.Format("Il file {0} non è corretto e non corrisponde ad una conservazione valida", fileName));
                            throw new Exception(string.Format("Il file {0} non è corretto e non corrisponde ad una conservazione valida", fileName));
                        }
                    }
                }
                return Content(JsonConvert.SerializeObject(new { status = result }));
            }, _loggerService);
        }


        [NoCache]
        [Authorize]
        public ActionResult PreservationToClose()
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                IpdaPreservationToCloseViewModel model = new IpdaPreservationToCloseViewModel();
                CustomerCompanyViewModel customerCompany = Session["idCompany"] as CustomerCompanyViewModel;
                IList<Preservation> preservationsToClose = PreservationService.PreservationToClose(customerCompany.CompanyId);
                model.PreservationsCount = preservationsToClose.Count;
                IList<string> files = new List<string>();
                foreach (Preservation preservation in preservationsToClose)
                {
                    if (Directory.GetFiles(preservation.Path, "IPDA*.tsd").Any())
                        continue;

                    string fileName = Directory.GetFiles(preservation.Path, "IPDA*.xml").FirstOrDefault();
                    if (!String.IsNullOrEmpty(fileName))
                        files.Add(fileName);
                }
                model.CloseFiles = files.ToArray();
                ViewBag.messageId = Guid.NewGuid().ToString();
                return View(model);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult DownloadPreservationFilesToClose()
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                string zipFile = string.Empty;
                try
                {
                    CustomerCompanyViewModel customerCompany = Session["idCompany"] as CustomerCompanyViewModel;
                    List<Preservation> preservations = PreservationService.PreservationToClose(customerCompany.CompanyId);
                    zipFile = CreatePreservationZipToDownload(preservations);
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

        [NoCache]
        [Authorize]
        [HttpGet]
        public ActionResult SignPreservationFilesToClose()
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                try
                {
                    CustomerCompanyViewModel customerCompany = Session["idCompany"] as CustomerCompanyViewModel;
                    List<Preservation> preservations = PreservationService.PreservationToClose(customerCompany.CompanyId);
                    Models.ArubaSignModel arubaSignModel = ArubaSignConfigurationHelper.GetArubaSignConfiguration(customerCompany.CompanyId, User.Identity.Name);
                    _logger.InfoFormat("Initializing signing with Aruba Automatic");
                    ISignService _signService = new SignService((info) => _logger.Info(info),
                                                           (err) => _logger.Error(err));

                    if (arubaSignModel == null)
                    {
                        return Json(ArubaSignConfigurationHelper.NO_CREDENTIALS, JsonRequestBehavior.AllowGet);
                    }
                    foreach (Preservation preservation in preservations)
                    {
                        SignDocuments(preservation, _signService, arubaSignModel);
                    }
                    return Json(ArubaSignConfigurationHelper.SIGN_COMPLETE, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Exception caught in process of signing: { ex.Message}", ex);
                    return Json(ArubaSignConfigurationHelper.SIGN_ERROR, JsonRequestBehavior.AllowGet);
                }
            }, _loggerService);
        }

        [NonAction]
        private void SignDocuments(Preservation preservation, ISignService _signService, Models.ArubaSignModel model)
        {
            string currentFilePath = Directory.GetFiles(preservation.Path, "IPDA*.xml").FirstOrDefault();
            if (currentFilePath != null)
            {
                //get file content
                byte[] content = System.IO.File.ReadAllBytes(currentFilePath);

                //sign document with aruba automatic                      
                VecompSoftware.Sign.ArubaSignService.ArubaSignModel arubaModel = new VecompSoftware.Sign.ArubaSignService.ArubaSignModel()
                {
                    DelegatedDomain = model.DelegatedDomain,
                    DelegatedPassword = model.DelegatedPassword,
                    DelegatedUser = model.DelegatedUser,
                    OTPPassword = model.OTPPassword,
                    OTPAuthType = model.OTPAuthType,
                    User = model.User,
                    CertificateId = model.CertificateId,
                    TSAUser = model.TSAUser,
                    TSAPassword = model.TSAPassword
                };

                content = _signService.SignDocument(arubaModel, content, Path.GetFileName(currentFilePath), true);

                //save signed file back to disk
                ByteArrayToFile(currentFilePath, content);

                //update CloseDate field so that we can close the preservation
                _preservationService.ClosePreservation(preservation.IdPreservation);
            }

        }

        [NonAction]
        private bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (FileStream fs = new FileStream($"{fileName}.p7m.tsd", FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Exception caught in process: {ex.Message}", ex);

                return false;
            }
        }

        [NonAction]
        private string CreatePreservationZipToDownload(List<Preservation> preservations)
        {
            string destination = Path.Combine(ConfigurationHelper.GetAppDataPath(), string.Format("Preservation_{0:yyyyMMddHHmmss}.zip", DateTime.Now));
            using (IWritableArchive archive = ArchiveFactory.Create(ArchiveType.Zip))
            {
                foreach (Preservation preservation in preservations)
                {
                    string file = IpdaUtil.GetIpdaXmlFile(preservation.Path);
                    archive.AddEntry(string.Concat(preservation.IdPreservation, "_", Path.GetFileName(file)), file);
                }
                archive.SaveTo(destination, CompressionType.Deflate);
            }
            return destination;
        }


        [NoCache]
        [HttpPost]
        public ActionResult ClosePreservation(Preservation preservation)
        {
            var service = new PreservationService();
            preservation = service.GetPreservation(preservation.IdPreservation, false);
            if (!preservation.CloseDate.HasValue)
            {
                if (service.VerifyExistingPreservation(preservation.IdPreservation))
                {
                    service.ClosePreservation(preservation.IdPreservation);
                    TempData["ErrorMessages"] = new string[] { "Conservazione eseguita e chiusa con successo" }.ToList();
                }
                else
                    TempData["ErrorMessages"] = service.ErrorMessages;
            }

            return RedirectToAction("PreservationCheck", "Preservation", new { id = preservation.IdPreservation });
        }


        [NoCache]
        [HttpPost]
        public ActionResult CloseOpenPreservations()
        {
            CustomerCompanyViewModel customerCompany = Session["idCompany"] as CustomerCompanyViewModel;
            var preservations = PreservationService.PreservationToClose(customerCompany.CompanyId);

            var service = new PreservationService();
            string res = "";

            foreach (var preservation in preservations)
            {
                if (!preservation.CloseDate.HasValue)
                {
                    if (service.VerifyExistingPreservation(preservation.IdPreservation))
                    {
                        service.ClosePreservation(preservation.IdPreservation);
                        string pattern = "Conservazione <b>{0}</b> del {1} <b>CHIUSA</b> in questa esecuzione.<br/>";
                        string filePath = Path.Combine(preservation.Path, preservation.Label);
                        res += string.Format(pattern, preservation.IdPreservation, preservation.StartDate);

                        //aggiunge informazioni sull'ipda
                        res += GetIpdaInfo(service.verifiedIpda);
                    }
                    else
                        res += String.Join("<br/>", service.ErrorMessages);
                }
            }

            return Json(new { result = res });
        }

        [NoCache]
        [Authorize]
        [HttpPost]
        public ActionResult PreservationUploadCloseFiles(IEnumerable<HttpPostedFileBase> files, string metaData, string messageId)
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
                    fileBlob.ExtraDescription = UnZipPreservationFile(fileBlob.FileName);
                    _uploadHelper.RemoveUploadedFile(fileBlob.FileName, messageId);
                }
                return Content(JsonConvert.SerializeObject(fileBlob), System.Net.Mime.MediaTypeNames.Text.Plain);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        [HttpPost]
        public ActionResult PreservationRemoveCloseFiles(string[] fileNames, string messageId)
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

        [NonAction]
        private string UnZipPreservationFile(string zipPath)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (System.IO.File.Exists(zipPath))
                {
                    using (Stream stream = System.IO.File.OpenRead(zipPath))
                    using (IReader reader = ReaderFactory.Open(stream))
                    {
                        while (reader.MoveToNextEntry())
                        {
                            if (!reader.Entry.IsDirectory)
                            {
                                string fileName = Path.GetFileName(reader.Entry.Key);
                                _logger.InfoFormat("UnzipPreservationFile -> Lettura file {0}", fileName);

                                string preservationIdStr = fileName.Split('_').FirstOrDefault();
                                if (Guid.TryParse(preservationIdStr, out Guid idPreservation))
                                {
                                    Preservation preservation = _preservationService.GetPreservation(idPreservation, false);
                                    if (preservation == null)
                                    {
                                        _logger.WarnFormat("Nessuna conservazione trovata con Id {0}", idPreservation);
                                        continue;
                                    }

                                    if (preservation.CloseDate.HasValue)
                                    {
                                        _logger.InfoFormat("UnzipPreservationFile -> Conservazione <b>{0}</b> del {1} <b>CHIUSA</b> il {2}.<br/>", preservation.IdPreservation, preservation.StartDate, preservation.CloseDate);
                                        sb.AppendFormat("Conservazione <b>{0}</b> del {1} <b>CHIUSA</b> il {2}.<br/>", preservation.IdPreservation, preservation.StartDate, preservation.CloseDate);
                                        continue;
                                    }

                                    string toSaveFile = Path.Combine(preservation.Path, fileName.Substring(fileName.IndexOf('_') + 1));
                                    if (System.IO.File.Exists(toSaveFile))
                                    {
                                        _logger.InfoFormat("UnzipPreservationFile -> File già presente!<br/>Il file verrà ignorato: {0}<br/>", Path.Combine(preservation.Path, preservation.Label, fileName.Substring(fileName.IndexOf('_') + 1)));
                                        sb.AppendFormat("File già presente!<br/>Il file verrà ignorato: {0}<br/>", Path.Combine(preservation.Path, preservation.Label, fileName.Substring(fileName.IndexOf('_') + 1)));
                                    }
                                    else
                                    {
                                        reader.WriteEntryToFile(toSaveFile, _extractionOptions);
                                        _logger.DebugFormat("UnzipPreservationFile -> file {0} saved correctly", toSaveFile);
                                    }

                                    string ipdaSignedFile = IpdaUtil.GetIpdaTsdFile(preservation.Path);
                                    if (string.IsNullOrEmpty(ipdaSignedFile))
                                    {
                                        _logger.WarnFormat("UnzipPreservationFile -> Nessun file IPDA firmato trovato nel percorso {0}", preservation.Path);
                                        continue;
                                    }

                                    ArchiveConfiguration archiveConfiguration = JsonConvert.DeserializeObject<ArchiveConfiguration>(preservation.Archive.PreservationConfiguration);
                                    if (!archiveConfiguration.CloseWithoutVerify)
                                    {
                                        _logger.InfoFormat("UnzipPreservationFile -> Chiusura Conservazione {0} con verify", preservation.IdPreservation);
                                        if (_preservationService.VerifyExistingPreservation(preservation.IdPreservation))
                                        {
                                            _logger.InfoFormat("UnzipPreservationFile -> Verifica conservazione {0} conclusa con esito positivo", preservation.IdPreservation);
                                        }
                                        else
                                        {
                                            _logger.InfoFormat("UnzipPreservationFile -> Verifica conservazione {0} conclusa con esito negativo", preservation.IdPreservation);
                                            sb.Append(string.Join("<br/>", _preservationService.ErrorMessages));
                                            continue;
                                        }
                                    }

                                    _preservationService.ClosePreservation(preservation.IdPreservation);
                                    string pattern = "Conservazione <b>{0}</b> del {1} <b>CHIUSA</b> in questa esecuzione.<br/>";
                                    sb.AppendFormat(pattern, preservation.IdPreservation, preservation.StartDate);

                                    //aggiunge informazioni sull'ipda
                                    sb.Append(GetIpdaInfo(_preservationService.verifiedIpda));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _logger.ErrorFormat("UnzipPreservationFile -> errore nello spacchettamento dello zip per la chiusura: {0}. La procedura non verrà interrotta", ex.Message);
                sb.Append(ex.Message);
            }
            return sb.ToString();
        }

        [NonAction]
        private string GetIpdaInfo(Ipda ipda)
        {
            StringBuilder ipdaInfoBulder = new StringBuilder();
            try
            {
                if (ipda == null)
                {
                    return "Informazioni da file chiusura non disponibili";
                }

                ipdaInfoBulder.AppendFormat("<br/><b>Elaborati n.{0} documenti - Tipo:'{1}' dalla data {2} alla data {3}</b>",
                  ipda.descGenerale.extraInfo.metadatiIntegrati.metadati.documenti.numero,
                  ipda.descGenerale.extraInfo.metadatiIntegrati.metadati.documenti.tipologia,
                  ipda.descGenerale.extraInfo.metadatiIntegrati.metadati.documenti.dal,
                  ipda.descGenerale.extraInfo.metadatiIntegrati.metadati.documenti.al
                );

                foreach (DgmiBlocco blocco in ipda.descGenerale.extraInfo.metadatiIntegrati.metadati.blocchi.bloccoList)
                {
                    ipdaInfoBulder.AppendFormat("<br/>- Gruppo <{0}> dal n.{1} al n.{2}", blocco.id, blocco.dal, blocco.al);
                }
                return ipdaInfoBulder.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error("UnzipPreservationFile -> errore nella lettura del file IPDA", ex);
                return "Informazioni da file chiusura non disponibili";
            }
        }
        #endregion
    }
}
