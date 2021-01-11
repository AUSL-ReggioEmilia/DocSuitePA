using BiblosDS.LegalExtension.AdminPortal.Helpers;
using BiblosDS.LegalExtension.AdminPortal.ViewModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Preservation.Services;
using BiblosDS.Library.Common.Services;
using Kendo.Mvc.UI;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VecompSoftware.BiblosDS.Model.Parameters;

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
            using (ServiceReferenceDocument.DocumentsClient client = new ServiceReferenceDocument.DocumentsClient())
            {
                DocumentContent result = client.GetDocumentContent(idDocument, null, Library.Common.Enums.DocumentContentFormat.Binary, null);
                return File(result.Blob, "text/xml");
            }            
        }

        [NoCache]
        [Authorize()]
        public ActionResult PreservationTaskEnable(Guid id)
        {
            PreservationTaskResponse query = new PreservationService().GetAllChildPreservationTasks(id, 0, 10);
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
            PreservationTaskResponse query = new PreservationService().GetAllChildPreservationTasks(id, 0, 10);
            PreservationTaskViewModel model = new PreservationTaskViewModel();
            model.Tasks = query.Tasks.OrderByDescending(x => x.EstimatedDate).ToList();
            model.VerifyTask = query.Tasks.FirstOrDefault(x => x.TaskType.Type == Library.Common.Objects.Enums.PreservationTaskTypes.Verify);
            model.HasVerifyTaskDefined = (model.VerifyTask != null);
            model.Archive = query.Tasks.First().Archive;
            model.ArchiveName = query.Tasks.First().Archive.Name;
            model.IdArchive = query.Tasks.First().Archive.IdArchive;
            model.Company = ArchiveService.GetCompanyFromArchive(model.IdArchive);
            model.HasCompanyDefined = (model.Company != null);

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
                if (string.IsNullOrEmpty(model.Archive.PreservationConfiguration))
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
            Preservation preservation = new PreservationService().GetPreservation(id, false);
            return View(preservation);
        }

        [NoCache]
        [Authorize()]
        public ActionResult ArchivePreservation(Guid id)
        {
            try
            {
                DocumentArchive model = ArchiveService.GetArchive(id);
                return View(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
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
            ArchiveConfiguration archiveConfiguration = JsonConvert.DeserializeObject<ArchiveConfiguration>(presTask.Archive.PreservationConfiguration);
            service.ResetErrorFlagForPreservationTask(idTask, true, archiveConfiguration.ForceAutoInc);
            return RedirectToAction("PreservationTaskDetails", new { id = idTask });
        }

        [NoCache]
        [Authorize()]
        public ActionResult ResetTask(Guid idTask)
        {
            PreservationService service = new PreservationService();
            PreservationTask task = service.GetPreservationTask(idTask);
            ArchiveConfiguration archiveConfiguration = JsonConvert.DeserializeObject<ArchiveConfiguration>(task.Archive.PreservationConfiguration);
            service.ResetPreservationTask(idTask, true, archiveConfiguration.ForceAutoInc);
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

        [NoCache]
        [Authorize()]
        public ActionResult ArchivePreservationConfiguration(Guid id)
        {
            ArchivePreservationConfigurationViewModel viewModel = ArchivePreservationConfigurationModel(id);
            return View(viewModel);
        }

        private ArchivePreservationConfigurationViewModel ArchivePreservationConfigurationModel(Guid id)
        {
            DocumentArchive archive = ArchiveService.GetArchive(id);
            ArchivePreservationConfigurationViewModel viewModel = new ArchivePreservationConfigurationViewModel() { Archive = archive, ArchiveConfiguration = new ArchiveConfiguration()};
            if (!string.IsNullOrEmpty(archive.PreservationConfiguration))
            {
                viewModel.ArchiveConfiguration = JsonConvert.DeserializeObject<ArchiveConfiguration>(archive.PreservationConfiguration);
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
        [HttpPost]
        public ActionResult SetArchivePreservationConfiguration(ArchivePreservationConfigurationViewModel model)
        {
            DocumentArchive archive = ArchiveService.GetArchive(model.Archive.IdArchive);
            archive.PreservationConfiguration = JsonConvert.SerializeObject(model.ArchiveConfiguration);
            ArchiveService.UpdateArchive(archive);
            return RedirectToAction("ArchivePreservationConfiguration", new { id = model.Archive.IdArchive });
        }

        [NoCache]
        [Authorize()]
        public ActionResult ExecutePurgePreservation(Guid id)
        {
            ViewData["idPreservation"] = id;
            return View();
        }

        [NoCache]
        [Authorize()]
        public ActionResult ClosePreservationWithoutDocuments(Guid idPreservationTask)
        {
            var service = new PreservationService();
            PreservationTask preservationTask = service.GetPreservationTask(idPreservationTask);
            service.SavePreservationTaskStatus(preservationTask, Library.Common.Objects.Enums.PreservationTaskStatus.Done, false, null);
            if (preservationTask.CorrelatedTasks != null)
            {
                foreach (PreservationTask correlatedtask in preservationTask.CorrelatedTasks)
                {
                    service.SavePreservationTaskStatus(correlatedtask, Library.Common.Objects.Enums.PreservationTaskStatus.Done, false, null);
                }
            }
            
            if (preservationTask.IdPreservation.HasValue)
            {
                service.ClosePreservation(preservationTask.IdPreservation.Value);
            }
            return RedirectToAction("Index", "Home");
        }

        [NoCache]
        [Authorize]
        public ActionResult ClosePreviousArchiveTasks(Guid idPreservationTask)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                PreservationService service = new PreservationService();
                PreservationTask preservationTask = service.GetPreservationTask(idPreservationTask);
                int taskYear = preservationTask.StartDocumentDate.Value.Year;
                IList<int> years = new List<int>() { (taskYear - 1) };
                if (preservationTask.StartDocumentDate.Value != new DateTime(taskYear, 1, 1))
                {
                    years.Add(taskYear);
                }
                ViewData["Years"] = years;
                TempData["IdPreservationTask"] = idPreservationTask;
                return PartialView("_ClosePreviousArchiveTasks");
            }, _loggerService);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ClosePreviousArchiveTasks(Guid idPreservationTask, int selectedTaskYear)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                PreservationService service = new PreservationService();
                PreservationTask preservationTask = service.GetPreservationTask(idPreservationTask);
                service.CreateClosePreviousPreservationTask(preservationTask, selectedTaskYear);
                return RedirectToAction("ArchivePreservationTask", new { id = preservationTask.Archive.IdArchive });
            }, _loggerService);
        }
    }

}
