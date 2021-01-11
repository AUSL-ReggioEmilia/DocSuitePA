using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.Helpers;
using BiblosDS.LegalExtension.AdminPortal.Infrastructure.Services.Common;
using BiblosDS.LegalExtension.AdminPortal.ViewModel;
using BiblosDS.LegalExtension.AdminPortal.ViewModel.Preservations;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.Services;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace BiblosDS.LegalExtension.AdminPortal.Controllers
{
    public class PreservationController : Controller
    {
        #region [ Fields ]
        private readonly ILog _logger = LogManager.GetLogger(typeof(PreservationController));
        private readonly ILogger _loggerService;
        private readonly PreservationService _preservationService;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public PreservationController()
        {
            _preservationService = new PreservationService();
            _loggerService = new LoggerService(_logger);
        }
        #endregion

        #region [ Methods ]
        [NoCache]
        [Authorize]
        public ActionResult PreservationCheck(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                Preservation preservation = _preservationService.GetPreservation(id, false);
                if (preservation == null)
                {
                    throw new Exception(string.Format("PreservationCheck -> conservazione con id {0} non trovata", id));
                }

                PreservationCheckViewModel model = new PreservationCheckViewModel()
                {
                    IdArchive = preservation.IdArchive,
                    IdPreservation = preservation.IdPreservation,
                    ArchiveName = preservation.Archive.Name,
                    ConservationDescription = string.Concat("dal <b>", preservation.StartDate.GetValueOrDefault().ToString("dd/MM/yyyy"), "</b> al <b>", preservation.EndDate.GetValueOrDefault().ToString("dd/MM/yyyy"), "</b>"),
                    Path = preservation.Path,
                    Manager = preservation.User != null ? string.Concat(preservation.User.Name, " ", preservation.User.Surname) : string.Empty,
                    CloseDateLabel = preservation.CloseDate.HasValue ? preservation.CloseDate.Value.ToString("dd/MM/yyyy") : "#",
                    LastVerifiedDateLabel = preservation.LastVerifiedDate.HasValue ? preservation.LastVerifiedDate.Value.ToString("dd/MM/yyyy") : "#",
                    PathExist = !string.IsNullOrEmpty(preservation.Path) && Directory.Exists(preservation.Path),
                    IsClosed = preservation.CloseDate.HasValue
                };

                if (model.PathExist)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(model.Path);
                    FileInfo[] files = directoryInfo.GetFiles("*verifica conservazione*", SearchOption.TopDirectoryOnly).OrderByDescending(t => t.LastWriteTime).ToArray();
                    model.VerifyFiles = files.Select(s => new PreservationCheckVerifyFileViewModel()
                    {
                        FileName = s.Name,
                        Success = !s.Name.Contains("negativo"),
                        DateCreatedLabel = string.Concat(s.LastWriteTime.ToShortDateString(), " ", s.LastWriteTime.ToShortTimeString())
                    }).ToList();
                }

                return View(model);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult PreservationCheckNotification(Guid id, string fileName)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                Preservation preservation = _preservationService.GetPreservation(id, false);
                string fullPath = Path.Combine(preservation.Path, fileName);
                if (!System.IO.File.Exists(fullPath))
                {
                    throw new FileNotFoundException("PreservationCheckNotification -> file non trovato", fullPath);
                }

                PreservationCheckNotificationViewModel model = new PreservationCheckNotificationViewModel()
                {
                    //TODO: Si può fare meglio, non è safe capire lo stato della notifica dal nome del file.
                    Success = !fileName.Contains("negativo"),
                    FileContent = new WindowFileContentViewModel()
                    {
                        Content = string.Join("<br />", System.IO.File.ReadAllLines(fullPath)),
                        Path = fullPath
                    }
                };
                return View(model);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult Check(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                Preservation preservation = _preservationService.GetPreservation(id, false);
                _preservationService.VerifyExistingPreservation(id);
                return RedirectToAction("PreservationCheck", "Preservation", new { id });
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        [HttpGet]
        public ActionResult DownloadPreservationPDA(Guid id, bool includeDocuments)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                var t = includeDocuments;
                Preservation preservation = _preservationService.GetPreservation(id, false);
                if (!Directory.Exists(preservation.Path))
                {
                    throw new DirectoryNotFoundException(string.Concat("DownloadPreservationPDA -> directory ", preservation.Path, " non trovata"));
                }

                string zipToDownload = _preservationService.GetZipPreservationPDA(preservation, includeDocuments);
                byte[] zipContent = System.IO.File.ReadAllBytes(zipToDownload);
                return File(zipContent, System.Net.Mime.MediaTypeNames.Application.Zip, Path.GetFileName(zipToDownload));
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult PreservationToClose()
        {
            return View();
        }

        [NoCache]
        [Authorize]
        public ActionResult GetPreservationsToClose([DataSourceRequest] DataSourceRequest request)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                DataSourceResult result = new DataSourceResult();
                CustomerCompanyViewModel customerCompany = Session["idCompany"] as CustomerCompanyViewModel;
                IList<Preservation> preservations = PreservationService.PreservationToClose(customerCompany.CompanyId);
                result.Total = preservations.Count;
                result.Data = preservations;
                return Json(result, JsonRequestBehavior.AllowGet);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult PreservationAudit()
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                PreservationAuditViewModel viewModel = new PreservationAuditViewModel();
                return View(viewModel);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult GetPreservationAudits([DataSourceRequest] DataSourceRequest request, DateTime? fromDate, DateTime? toDate)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                DataSourceResult result = new DataSourceResult();
                CustomerCompanyViewModel customerCompany = Session["idCompany"] as CustomerCompanyViewModel;


                bool sortingDescending = request.Sorts.Any(x => x.Member == "ActivityDate" && x.SortDirection == ListSortDirection.Descending);
                if (toDate.HasValue)
                {
                    toDate = toDate.Value.AddDays(1).AddMinutes(-1);
                }

                Guid? idPreservationJournalActivity = null;
                if (request.Filters.Count > 0)
                {
                    if (request.Filters.First() is FilterDescriptor descriptor && descriptor.Member == "ActivityName")
                    {
                        idPreservationJournalActivity = Guid.Parse(descriptor.Value.ToString());
                    }
                }

                ICollection<PreservationJournaling> audits = _preservationService.GetPreservationJournalings(null, null, fromDate, toDate, idPreservationJournalActivity, customerCompany.CompanyId, (request.Page - 1) * request.PageSize, request.PageSize, out int journalingsInArchive, false, sortingDescending);
                result.Total = journalingsInArchive;
                result.Data = audits.Select(s => new PreservationAuditGridViewModel()
                {
                    ActivityDate = s.DateActivity,
                    ActivityName = s.PreservationJournalingActivity?.Description,
                    ActivityUser = string.Concat(s.User?.Name, " ", s.User.Surname),
                    Description = string.IsNullOrEmpty(s.Notes) && s.Preservation != null ? s.Preservation.Label : s.Notes,
                    IdPreservation = s.IdPreservation
                });
                return Json(result, JsonRequestBehavior.AllowGet);
            }, _loggerService);
        }

        [NoCache]
        [Authorize]
        public ActionResult GetPreservationAuditType()
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                ICollection<PreservationJournalingActivity> preservationJournalingActivities = _preservationService.GetPreservationJournalingActivities(null, false);
                return Json(preservationJournalingActivities, JsonRequestBehavior.AllowGet);
            }, _loggerService);
        }
        #endregion
    }
}