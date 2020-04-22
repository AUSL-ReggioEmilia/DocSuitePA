
using BiblosDS.Cloud;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.AwardBatches;
using BiblosDS.LegalExtension.AdminPortal.Helpers;
using BiblosDS.LegalExtension.AdminPortal.ViewModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Enums;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Preservation.Services;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Utility;
using BiblosDS.Library.Helper;
using Kendo.Mvc.UI;
using log4net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BiblosDS.LegalExtension.AdminPortal.Controllers
{

    public partial class HomeController
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(HomeController));
        [NoCache]
        [Authorize()]
        public ActionResult ArchiveAdmin()
        {
            return View();
        }

        [NoCache]
        [Authorize()]
        public ActionResult ArchivePreservationTask(Guid id)
        {
            var viewModel = ArchivePreservationModel(id);
            return View(viewModel);
        }

        private PreservationTaskViewModel ArchivePreservationModel(Guid id)
        {
            var service = new PreservationService();
            var archive = ArchiveService.GetArchive(id);
            var schedule = service.GetPreservationScheduleWithinArchive(id);
            var SelectedScheduleIndex = -1;
            var schedulePeriod = GetArchivePeriods(id);
            var tasks = service.GetPreservationTasks(new System.ComponentModel.BindingList<DocumentArchive> { new DocumentArchive(id) }, 0, 2);
            DateTime nextPreservationDate = DateTime.Now;
            PreservationSchedule objSched = null;
            if (schedule != null)
            {
                objSched = schedulePeriod.Where(x => x.IdPreservationSchedule == schedule.IdPreservationSchedule).FirstOrDefault();
                if (objSched != null)
                    SelectedScheduleIndex = schedulePeriod.IndexOf(objSched);
            }

            if (tasks.Tasks.Count > 0)
            {
                nextPreservationDate = tasks.Tasks.First().EndDocumentDate.Value.AddDays(1);
            }
            var viewModel = new PreservationTaskViewModel { IdArchive = id, ArchiveName = archive.Name, SelectedScheduleIndex = SelectedScheduleIndex, PeriodSchedulers = schedulePeriod, NextPreservationTaskStartDocumentDate = nextPreservationDate };
            return viewModel;
        }

        [NoCache]
        [Authorize()]
        public ActionResult ArchiveCompany(Guid id)
        {
            var service = new PreservationService();
            var companies = service.GetCompanies();
            var archive = ArchiveService.GetArchive(id);
            var archiveCompany = new Library.Common.Objects.ArchiveCompany() { Archive = archive, IdArchive = id };
            ArchiveCompanyViewModel model = new ArchiveCompanyViewModel() { ArchiveCompany = archiveCompany, Companies = companies.ToList() };

            return View(model);
        }

        //TODO: validare campi workingDir, xmlFileTemplatePath, templateXSLTFile e awardBatchXSLTFile
        [NoCache]
        [Authorize()]
        [HttpPost]
        public ActionResult ArchiveCompany(Guid archiveCompanyIdArchive, Guid archiveCompanySetIdCompany, string workingDir, string xmlFileTemplatePath, string templateXSLTFile, string awardBatchXSLTFile)
        {
            ArchiveCompanyViewModel model = new ArchiveCompanyViewModel();
            var service = new PreservationService();
            var company = service.GetCompany(archiveCompanySetIdCompany);
            ArchiveCompany archiveCompany = new Library.Common.Objects.ArchiveCompany() { Archive = ArchiveService.GetArchive(archiveCompanyIdArchive), IdArchive = archiveCompanyIdArchive, IdCompany = archiveCompanySetIdCompany, Company = company, CompanyName = company.CompanyName, WorkingDir = workingDir, TemplateXSLTFile = templateXSLTFile, XmlFileTemplatePath = xmlFileTemplatePath, AwardBatchXSLTFile = awardBatchXSLTFile };

            model = new ArchiveCompanyViewModel() { ArchiveCompany = archiveCompany, Companies = new List<Company>() { company } };
            try
            {
                ArchiveService.AddArchiveCompany(model.ArchiveCompany);

            }
            catch (Exception e)
            {
                View(model);
            }
            return RedirectToAction("Index");

        }

        [NoCache]
        [Authorize()]
        [HttpPost]
        public ActionResult PreservationVerifyDoAction(Guid id)
        {
            new PreservationService().DeletePreservationVerifyResetTaskErrors(id);
            return Json(new { });
        }

        [NoCache]
        [Authorize()]
        public ActionResult ExistingPreservationVerifyDoAction(Guid id)
        {
            var service = new PreservationService();
            Preservation pres = service.GetPreservation(id);
            service.ArchiveConfigFile = ConfigurationHelper.GetArchiveConfigurationFilePath(pres.Archive.Name);
            service.VerifyExistingPreservation(id);
            TempData["ErrorMessages"] = service.ErrorMessages;

            return RedirectToAction("PreservationDetails", new { id = id });
        }

        [NoCache]
        [Authorize()]
        public ActionResult ExecutePreservationTask(Guid id)
        {
            ViewData["idTask"] = id;
            return View();
        }

        [NoCache]
        [Authorize()]
        public ActionResult DownloadFile(Guid idDocument)
        {
            ServiceReferenceDocument.DocumentsClient client = new ServiceReferenceDocument.DocumentsClient();
            var result = client.GetDocumentContent(idDocument, null, Library.Common.Enums.DocumentContentFormat.Binary, null);
            return File(result.Blob, "text/xml");
        }

        [NoCache]
        [Authorize()]
        public ActionResult MountPreservationDevice(Guid idPreservation)
        {
            try
            {
                var preservation = new PreservationService().GetPreservation(idPreservation, false);
                var company = ArchiveService.GetCompanyFromArchive(preservation.Archive.IdArchive);
                CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting(CloudDriveManager.STORAGE_ACCOUNT_SETTING);
                CloudBlobClient blobClient = account.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(company.IdCompany.ToString());
                var blob = container.GetPageBlobReference("Gianni_Mancini.vhd");

                var permissions = container.GetPermissions();
                permissions.SharedAccessPolicies.Remove("readonly");
                permissions.SharedAccessPolicies.Add("readonly", new SharedAccessPolicy()
                {
                    Permissions = SharedAccessPermissions.Read
                });
                container.SetPermissions(permissions, new BlobRequestOptions()
                {
                    // fail if someone else has already changed the container before we do
                    AccessCondition = AccessCondition.IfMatch(container.Properties.ETag)
                });

                var sasWithIdentifier = blob.GetSharedAccessSignature(new SharedAccessPolicy()
                {
                    SharedAccessExpiryTime = DateTime.UtcNow + TimeSpan.FromDays(1)
                }, "readonly");


                var drive = blob.Uri.AbsoluteUri + sasWithIdentifier;


                //CloudDriveManager manager = new CloudDriveManager();
                //manager.CreateDriveFromUrl(preservation.Path);      


                //var drive = manager.Mount();

                return Json(new { drive = drive });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.ToString() });
            }
        }

        public static string ParseAzureStorageName(string name)
        {
            var res = Regex.Replace(name, @"[^A-Za-z0-9]+", "_");
            return res.Substring(0, Math.Min(res.Length, 60));
        }

        [NoCache]
        [Authorize()]
        public ActionResult DownloadPreservationDevice(Guid id)
        {
            try
            {
                var pres = new PreservationService().GetPreservation(id, false);
                var company = ArchiveService.GetCompanyFromArchive(pres.Archive.IdArchive);

                CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting(CloudDriveManager.STORAGE_ACCOUNT_SETTING);
                CloudBlobClient blobStorage = account.CreateCloudBlobClient();
                blobStorage.ReadAheadInBytes = 0;

                CloudBlobContainer container = blobStorage.GetContainerReference(company.IdCompany.ToString());
                CloudPageBlob pageBlob = container.GetPageBlobReference(string.Format("{0}.vhd", ParseAzureStorageName(pres.Archive.PathPreservation.ToStringExt() + company.CompanyName)));

                // Get the length of the blob
                pageBlob.FetchAttributes();
                long vhdLength = pageBlob.Properties.Length;
                long totalDownloaded = 0;
                Console.WriteLine("Vhd size:  " + Megabytes(vhdLength));

                // Create a new local file to write into
                MemoryStream fileStream = new MemoryStream();
                fileStream.SetLength(vhdLength);

                // Download the valid ranges of the blob, and write them to the file
                IEnumerable<PageRange> pageRanges = pageBlob.GetPageRanges();
                BlobStream blobStream = pageBlob.OpenRead();

                foreach (PageRange range in pageRanges)
                {
                    // EndOffset is inclusive... so need to add 1
                    int rangeSize = (int)(range.EndOffset + 1 - range.StartOffset);

                    // Chop range into 4MB chucks, if needed
                    for (int subOffset = 0; subOffset < rangeSize; subOffset += FourMegabyteAsBytes)
                    {
                        int subRangeSize = Math.Min(rangeSize - subOffset, FourMegabyteAsBytes);
                        blobStream.Seek(range.StartOffset + subOffset, SeekOrigin.Begin);
                        fileStream.Seek(range.StartOffset + subOffset, SeekOrigin.Begin);

                        Console.WriteLine("Range: ~" + Megabytes(range.StartOffset + subOffset)
                                          + " + " + PrintSize(subRangeSize));
                        byte[] buffer = new byte[subRangeSize];

                        blobStream.Read(buffer, 0, subRangeSize);
                        fileStream.Write(buffer, 0, subRangeSize);
                        totalDownloaded += subRangeSize;
                    }
                }

                return new FileStreamResult(fileStream, "*.vhd") { FileDownloadName = "Conservazione.vhd" };

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.ToString() });
            }
        }

        private static int PageBlobPageSize = 512;
        private static int OneMegabyteAsBytes = 1024 * 1024;
        private static int FourMegabytesAsBytes = 4 * OneMegabyteAsBytes;
        private static int FourMegabyteAsBytes = 4 * OneMegabyteAsBytes;

        private static string PrintSize(long bytes)
        {
            if (bytes >= 1024 * 1024) return (bytes / 1024 / 1024).ToString() + " MB";
            if (bytes >= 1024) return (bytes / 1024).ToString() + " kb";
            return (bytes).ToString() + " bytes";
        }

        private static string Megabytes(long bytes)
        {
            return (bytes / OneMegabyteAsBytes).ToString() + " MB";
        }
        private static long RoundUpToPageBlobSize(long size)
        {
            return (size + PageBlobPageSize - 1) & ~(PageBlobPageSize - 1);
        }

        [NoCache]
        [HttpPost]
        [Authorize()]
        public ActionResult UnMountPreservationDevice(Guid idPreservation)
        {
            try
            {
                var preservation = new PreservationService().GetPreservation(idPreservation, false);
                CloudDriveManager manager = new CloudDriveManager();
                manager.CreateDriveFromUrl(preservation.Path);
                var drive = manager.Unmount();
                return Json(new { drive = drive, url = manager.VHD_Url });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.ToString() });
            }
        }

        [NoCache]
        [Authorize()]
        public ActionResult PreservationTaskEnable(Guid id)
        {
            var query = new PreservationService().GetAllChildPreservationTasks(id, 0, 10);
            foreach (var item in query.Tasks)
            {
                new PreservationService().EnablePreservationTask(item.IdPreservationTask);
            }
            return RedirectToAction("PreservationTaskDetails", new { id = id });
        }

        [NoCache]
        [Authorize()]
        public ActionResult PreservationTaskDetails(Guid id)
        {
            var query = new PreservationService().GetAllChildPreservationTasks(id, 0, 10);
            PreservationTaskViewModel model = new PreservationTaskViewModel();
            model.Tasks = query.Tasks.OrderByDescending(x => x.EstimatedDate).ToList();
            var archive = query.Tasks.First().Archive;
            model.Archive = archive;
            model.ArchiveName = query.Tasks.First().Archive.Name;
            model.IdArchive = query.Tasks.First().Archive.IdArchive;
            model.Company = ArchiveService.GetCompanyFromArchive(model.IdArchive);

            foreach (PreservationTask task in model.Tasks)
            {
                if (task.IdPreservation != null)
                {
                    try
                    {
                        string preservationPath = new PreservationService().GetPreservationPathByIdTask(task.IdPreservationTask);
                        if (Directory.Exists(preservationPath))
                        {
                            DirectoryInfo path = new DirectoryInfo(preservationPath);

                            logger.InfoFormat("caricamento file di chiusura {0}", path.FullName);

                            task.PreservationCloseFile = path.GetFiles("CHIUSURA*", SearchOption.TopDirectoryOnly).First().FullName;
                        }
                    }
                    catch (Exception ex)
                    {
                        task.PreservationCloseFile = null;
                    }
                }
            }

            model.HasArchiveConfigurationFile = false;
            if (!string.IsNullOrEmpty(model.ArchiveName))
            {
                string pathConfiguration = ConfigurationHelper.GetArchiveConfigurationFilePath(model.ArchiveName);

                if (!System.IO.File.Exists(pathConfiguration))
                {
                    logger.InfoFormat("L'archivio {0} non è stato configurato", model.Tasks.FirstOrDefault().Archive.Name);
                    model.HasArchiveConfigurationFile = false;
                }
                else
                {
                    model.HasArchiveConfigurationFile = true;
                }
            }

            return View(model);
        }

        [NoCache]
        [Authorize()]
        public ActionResult DownloadPreservationCloseFile(Guid idPreservation)
        {
            PreservationFileInfoResponse res = new PreservationService().GetPreservationClosingFileInfo(idPreservation);
            return File(res.File, "application/txt");
        }

        [NoCache]
        [Authorize()]
        public ActionResult PreservationDetails(Guid id)
        {
            var preservation = new PreservationService().GetPreservation(id, false);
            return View(preservation);
        }

        [NoCache]
        [Authorize()]
        public ActionResult ArchivePreservation(Guid id)
        {
            DocumentArchive model = null;
            try
            {
                model = ArchiveService.GetArchive(id);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return View(model);
        }

        [NoCache]
        [HttpPost]
        public ActionResult PreservationUploadFiles(Guid idPreservation, string preservationPath)
        {
            var errorMessage = string.Empty;

            if (Request.Files != null)
            {
                try
                {
                    var service = new PreservationService();
                    var preservation = service.GetPreservation(idPreservation, false);
                    string fileName, type;
                    HttpPostedFileBase file;

                    foreach (string formName in Request.Files)
                    {
                        file = Request.Files[formName];
                        fileName = Path.Combine(preservationPath, Path.GetFileName(file.FileName));
                        type = file.ContentType;

                        file.SaveAs(fileName);

                        //try
                        //{
                        //    if (fileName.ToUpper().Contains("CHIUSURA_"))
                        //    {
                        //        if (!checkPreservationFilesIntegrity(idPreservation, fileName, null, false))
                        //            throw new Exception("La verifica del file di chiusura ha avuto esito negativo");
                        //    }
                        //}
                        //catch
                        //{
                        //    throw;
                        //}
                    }


                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    errorMessage = ex.ToString();
                }
            }
            else
            {
                errorMessage = "Nessun file passato.";
            }

            return Content(errorMessage);
        }

        [NoCache]
        [HttpPost]
        public ActionResult ClosePreservation(Preservation preservation)
        {
            var service = new PreservationService();
            preservation = service.GetPreservation(preservation.IdPreservation, false);
            if (!preservation.CloseDate.HasValue)
            {
                service.ClosePreservation(preservation.IdPreservation);
            }
            return RedirectToAction("PreservationDetails", new { id = preservation.IdPreservation });
        }

        [NoCache]
        [HttpPost]
        public ActionResult PreservationRemoveUploadedFiles(string[] fileNames, string preservationPath)
        {
            string errorMessage = "";
            // The parameter of the Remove action must be called "fileNames"
            try
            {
                foreach (var fullName in fileNames)
                {
                    var physicalPath = Path.Combine(preservationPath, Path.GetFileName(fullName));
                    System.IO.File.Delete(physicalPath);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                errorMessage = ex.ToString();
            }
            // Return an empty string to signify success
            return Content(errorMessage);
        }


        [NoCache]
        [Authorize()]
        public ActionResult SetArchiveDefaultSchedule(Guid idArchive, Guid taskPeriodSetId)
        {
            var service = new PreservationService();
            service.AddPreservationScheduleArchive(new PreservationScheduleArchive { IdSchedule = taskPeriodSetId, IdArchive = idArchive });
            return View("ArchivePreservationTask", ArchivePreservationModel(idArchive));
        }

        [NoCache]
        [Authorize()]
        public ActionResult CreatePreservationTask(Guid idArchive, Guid? taskPeriodId, int taskYear, int taskMonth, int taskDay, bool onlyOneTask, bool isEnabled)
        {
            try
            {
                if (!taskPeriodId.HasValue)
                {
                    throw new Exception("La Periodicità del Task è necessaria");
                }
                var service = new PreservationService();
                var selectedSchedule = service.GetSchedule(taskPeriodId).SingleOrDefault<PreservationSchedule>();
                if (selectedSchedule != null)
                {
                    if (onlyOneTask)
                        service.CreatePreservationTask(idArchive, selectedSchedule, new DateTime(taskYear, taskMonth, taskDay), isEnabled);
                    else
                    {

                        /*
                         * Tipologie frequenze:
                         * 0 - cadenzata
                         * 1 - giornaliera
                         * 2 - settimanale
                         * 3 - mensile
                         * 4 - annuale
                         */


                        var taskToCreate = new List<PreservationTask>();
                        if (selectedSchedule.FrequencyType == 3) //Mensile.
                        {
                            for (var i = 1; i < 13; i++)
                            {
                                if (i < taskMonth)
                                    continue;

                                taskToCreate.AddRange(service.getTasksFromSchedulerPeriod(selectedSchedule.Period, i, taskYear, taskDay, onlyOneTask));
                            }
                        }
                        else
                        {
                            taskToCreate.AddRange(service.getTasksFromSchedulerPeriod(selectedSchedule.Period, taskMonth, taskYear, taskDay, onlyOneTask));
                        }

                        foreach (var task in taskToCreate)
                        {
                            task.TaskType = new PreservationTaskType { IdPreservationTaskType = Guid.Empty, Type = Library.Common.Objects.Enums.PreservationTaskTypes.Verify };
                            task.Archive = new DocumentArchive(idArchive);
                            task.Enabled = isEnabled;

                            task.CorrelatedTasks = new BindingList<PreservationTask>();
                            task.CorrelatedTasks.Add(new PreservationTask
                            {
                                TaskType = new PreservationTaskType { IdPreservationTaskType = Guid.Empty, Type = Library.Common.Objects.Enums.PreservationTaskTypes.Preservation },
                                Archive = new DocumentArchive(idArchive),
                                StartDocumentDate = task.StartDocumentDate,
                                EndDocumentDate = task.EndDocumentDate,
                                EstimatedDate = task.EstimatedDate,
                                Enabled = task.Enabled
                            });
                        }

                        //service.RemovePendigPreservation(idArchive);
                        var result = service.CreatePreservationTask(new BindingList<PreservationTask>(taskToCreate));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("ArchivePreservationTask", new { id = idArchive });
        }

        [NoCache]
        [Authorize()]
        public ActionResult ResetTaskErrors(Guid idTask)
        {
            PreservationService service = new PreservationService();
            PreservationTask presTask = service.GetPreservationTask(idTask);
            service.ArchiveConfigFile = ConfigurationHelper.GetArchiveConfigurationFilePath(presTask.Archive.Name);            
            service.ResetErrorFlagForPreservationTask(idTask, true);
            return RedirectToAction("PreservationTaskDetails", new { id = idTask });
        }

        [NoCache]
        [Authorize()]
        public ActionResult ResetTask(Guid idTask)
        {
            PreservationService service = new PreservationService();
            PreservationTask task = service.GetPreservationTask(idTask);
            service.ArchiveConfigFile = ConfigurationHelper.GetArchiveConfigurationFilePath(task.Archive.Name);
            service.ResetPreservationTask(idTask, true);
            return RedirectToAction("PreservationTaskDetails", new { id = idTask });
        }        

        [NoCache]
        public ActionResult DeletePreservationTask(Guid id, [DataSourceRequest] DataSourceRequest request)
        {
            var service = new PreservationService();
            //Guid idPreservationTask = Guid.Parse(id);
            Guid idPreservationTask = id;
            var preservationTask = service.GetPreservationTask(idPreservationTask);
            Guid idArchive = preservationTask.Archive.IdArchive;
            if (preservationTask.ExecutedDate == null && preservationTask.IdPreservation == null)
            {

                service.DeletePreservationTask(preservationTask.IdPreservationTask);

            }
            return RedirectToAction("ArchivePreservationTask", new { id = idArchive });
        }
        private List<PreservationSchedule> GetArchivePeriods(Guid archiveId)
        {
            var retval = new List<PreservationSchedule>();

            try
            {
                var service = new PreservationService();
                var periods = service.GetSchedule();

                //PreservationSchedule previouslyUsedSchedule = null;

                var selSchedule = service.GetPreservationScheduleWithinArchive(archiveId);
                if (selSchedule != null)
                    foreach (var item in periods)
                    {
                        item.IsArchiveDefault = item.IdPreservationSchedule == selSchedule.IdPreservationSchedule;
                    }
                retval = new List<PreservationSchedule>(periods);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return retval;
        }



        private DateTime findNextValidWeekDay(DateTime baseDate, bool includeBaseDateDay = false, params int[] validWeekDays)
        {
            DateTime retval = DateTime.MinValue;

            if (baseDate != null && baseDate != DateTime.MinValue && baseDate != DateTime.MaxValue)
            {
                try
                {
                    IEnumerable<DayOfWeek> validDays;

                    if (validWeekDays != null && validWeekDays.Any())
                    {
                        validDays = validWeekDays.Cast<DayOfWeek>();
                    }
                    else
                    {
                        validDays = Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().Where(x => x != DayOfWeek.Saturday && x != DayOfWeek.Sunday);
                    }

                    if (includeBaseDateDay && validDays.Contains(baseDate.DayOfWeek))
                    {
                        retval = baseDate;
                    }
                    else
                    {
                        DateTime newDate;
                        for (newDate = baseDate.AddDays(1); !validDays.Contains(newDate.DayOfWeek); newDate = newDate.AddDays(1))
                        {
                            //NESSUNA ULTERIORE OPERAZIONE DA FARE.
                        }
                        retval = newDate;
                    }
                }
                catch { }
            }

            return retval;
        }

        /*
        private IEnumerable<PreservationTask> getTasksFromSchedulerPeriod(string encodedPeriod, int baseMonth, int baseYear)
        {
          var retval = new List<PreservationTask>();

          if (!string.IsNullOrWhiteSpace(encodedPeriod))
          {
            DateTime startDate = new DateTime(baseYear, baseMonth, 1), endDate;
            PreservationTask toAdd = new PreservationTask();
            var splittedPeriods = encodedPeriod.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string[] splittedMonthAndDay;
            int month = 0, day = 0, lastMonthDay;

            foreach (var period in splittedPeriods)
            {
              splittedMonthAndDay = period.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

              if (splittedMonthAndDay.Count() > 1)
              {
                try
                {
                  try
                  {
                    month = int.Parse(splittedMonthAndDay.First());
                    day = int.Parse(splittedMonthAndDay.Last());
                  }
                  catch { throw new Exception("Configurazione non corretta per il periodo dello scadenziario."); }

                  endDate = new DateTime(baseYear, month, day);
                }
                catch (ArgumentOutOfRangeException)
                {
                  //Se si verifica un'eccezione di tipo "ArgumentOutOfRangeException" è perchè
                  //l'ultimo giorno del mese configurato in "encodedPeriod" è superiore all'ultimo
                  //giorno del mese reale.
                  lastMonthDay = new DateTime(baseYear, baseMonth, 1).AddMonths(1).AddHours(-1).Day;
                  endDate = new DateTime(baseYear, baseMonth, lastMonthDay);
                }
              }
              else
              {
                try
                {
                  try
                  {
                    day = int.Parse(period);
                  }
                  catch { throw new Exception("Configurazione non corretta per il periodo dello scadenziario."); }

                  endDate = new DateTime(baseYear, baseMonth, day);
                }
                catch (ArgumentOutOfRangeException)
                {
                   //Se si verifica un'eccezione di tipo "ArgumentOutOfRangeException" è perchè
                   //l'ultimo giorno del mese configurato in "encodedPeriod" è superiore all'ultimo
                   //giorno del mese reale.
                  lastMonthDay = new DateTime(baseYear, baseMonth, 1).AddMonths(1).AddHours(-1).Day;
                  endDate = new DateTime(baseYear, baseMonth, lastMonthDay);
                }
              }

              //considera periodo successivo
              if (endDate <= startDate)
                continue;

              toAdd = new PreservationTask { StartDocumentDate = startDate, EndDocumentDate = endDate, EstimatedDate = findNextValidWeekDay(endDate) };
              startDate = endDate.AddDays(1);

              retval.Add(toAdd);
            }
          }

          return retval;
        }
        */

        private bool checkPreservationFilesIntegrity(Guid idPreservation, string closingFilePath, string indexFilePath, bool checkAgainstIndexFile)
        {
            var areValids = false;

            try
            {
                if (idPreservation == Guid.Empty || (string.IsNullOrWhiteSpace(closingFilePath) && string.IsNullOrWhiteSpace(indexFilePath)))
                    throw new Exception("Parametri di input insufficienti.");

                if (!string.IsNullOrWhiteSpace(closingFilePath) && System.IO.File.Exists(closingFilePath))
                {
                    var lines = System.IO.File.ReadAllLines(closingFilePath);
                    var element = lines
                        .FirstOrDefault<string>(x => x.Trim().ToUpper().StartsWith("IDENTIFICATIVO CONSERVAZIONE SOSTITUTIVA"));

                    if (element == null)
                        throw new Exception("Il formato del file di chiusura non è valido.");

                    var txtGuid = element
                        .Trim()
                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .LastOrDefault<string>();
                    Guid id;

                    if (string.IsNullOrWhiteSpace(txtGuid) || !Guid.TryParse(txtGuid, out id))
                        throw new Exception("Impossibile recuperare l'identificativo della conservazione a partire dal file di chiusura.");

                    areValids = (id == idPreservation);

                    if (checkAgainstIndexFile)
                    {
                        if (!string.IsNullOrWhiteSpace(indexFilePath) && System.IO.File.Exists(indexFilePath))
                        {
                            //TODO: Implementare verifica integrità & coerenza tra file INDICE e file CHIUSURA.
                        }
                        else
                        {
                            throw new Exception("Impossibile confrontare il file di chiusura con il file indice.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return areValids;
        }


        private JsonResult ThrowJSONError(Exception e)
        {
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            //Log your exception
            return Json(new { Message = e.Message });
        }

        [NoCache]
        [Authorize()]
        public ActionResult ArchivePreservationConfiguration(Guid id)
        {
            ArchivePreservationConfigurationViewModel viewModel = ArchivePreservationConfigurationModel(id);
            return View(viewModel);
        }

        private ArchivePreservationConfigurationViewModel ArchivePreservationConfigurationModel(Guid id)
        {
            var archive = ArchiveService.GetArchive(id);
            string path = ConfigurationHelper.GetArchiveConfigurationFilePath(archive.Name);
            ArchivePreservationConfigurationViewModel viewModel = new ArchivePreservationConfigurationViewModel() { Archive = archive, ArchiveConfiguration = new ArchiveConfiguration()};
            if (System.IO.File.Exists(path))
            {
                viewModel.ArchiveConfiguration = PreservationHelper.GetArchiveConfiguration(path);
            }

            if (viewModel.Archive.FiscalDocumentType == null)
            {
                viewModel.Archive.FiscalDocumentType = string.Empty;
            }
            if (viewModel.ArchiveConfiguration.PreservationLimitTaskToDocumentDate == null)
            {                
                viewModel.ArchiveConfiguration.PreservationLimitTaskToDocumentDate = DateTime.Now;
            }
            return viewModel;
        }

        [NoCache]
        [Authorize]
        public ActionResult SetArchivePreservationConfiguration(Guid idArchive, int firstConservationDay, int firstConservationMonth, int firstConservationYear, bool forceAutoInc, bool autoClose, bool generateTask, bool closeWithoutVerify, bool checkTsd)
        {
            DocumentArchive archive = ArchiveService.GetArchive(idArchive);

            DateTime lowerLimit = new DateTime(firstConservationYear, firstConservationMonth, firstConservationDay).Date.ToLocalTime();

            ArchiveConfiguration archiveConfiguration = new ArchiveConfiguration() {IdArchive=idArchive, PreservationLimitTaskToDocumentDate=lowerLimit, ForceAutoInc = forceAutoInc, PreservationAutoClose = autoClose, AutoGeneratedNextTask = generateTask, CloseWithoutVerify = closeWithoutVerify, CheckTsd = checkTsd };

            string path = ConfigurationHelper.GetArchiveConfigurationFilePath(archive.Name);

            PreservationHelper.UpdateArchiveConfiguration(archiveConfiguration, path);                 

            return RedirectToAction("ArchivePreservationConfiguration", new { id = idArchive }); ;
        }

        [NoCache]
        [Authorize()]
        public ActionResult ExecutePurgePreservation(Guid id)
        {
            ViewData["idPreservation"] = id;
            return View();
        }
    }

}
