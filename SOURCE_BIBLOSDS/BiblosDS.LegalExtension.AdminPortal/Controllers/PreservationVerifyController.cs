using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Preservations;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Services.Preservations.Interactors;
using BiblosDS.LegalExtension.AdminPortal.Helpers;
using BiblosDS.LegalExtension.AdminPortal.Infrastructure.Services.Common;
using BiblosDS.LegalExtension.AdminPortal.Models;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.Services;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Utility;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BiblosDS.LegalExtension.AdminPortal.Controllers
{
    [Authorize]
    public class PreservationVerifyController : Controller
    {
        #region [ Fields ]
        private readonly ILog _logger = LogManager.GetLogger(typeof(PreservationVerifyController));
        private readonly ILogger _loggerService;
        private readonly PreservationService _preservationService;

        static ConcurrentDictionary<string, ICollection<PreservationVerifyJob>> _cache_jobs = new ConcurrentDictionary<string, ICollection<PreservationVerifyJob>>();
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public PreservationVerifyController()
        {
            _preservationService = new PreservationService();
            _loggerService = new LoggerService(_logger);
        }
        #endregion

        #region [ Methods ]
        [HttpGet]
        public ActionResult Index()
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                PreservationVerifyIndexModel model = new PreservationVerifyIndexModel();
                if (AzureService.GetSettingValue("DBAdminLoginConnection") == "false")
                {
                    DocumentCondition conditions = new DocumentCondition();

                    List<DocumentSortCondition> sortConditions = new List<DocumentSortCondition>();
                    conditions.DocumentAttributeConditions = new System.ComponentModel.BindingList<DocumentCondition>();
                    conditions.DocumentAttributeConditions.Add(new DocumentCondition() { Name = "IsLegal", Value = 1, Operator = Library.Common.Enums.DocumentConditionFilterOperator.IsEqualTo, Condition = Library.Common.Enums.DocumentConditionFilterCondition.And });
                    sortConditions.Add(new DocumentSortCondition { Name = "Name", Dir = "ASC" });

                    model.archives = ArchiveService.GetArchives(0, int.MaxValue, conditions, sortConditions, out int total).ToList();
                }
                else
                {
                    model.archives = UserArchive.GetUserArchivesPaged(User.Identity.Name, 0, int.MaxValue, out int total);
                }

                return View(model);
            }, _loggerService);
        }

        [HttpGet]
        public ActionResult List()
        {
            return View(new PreservationVerifyListModel
            {
                verifyFolder = Path.Combine(Server.MapPath("~/Reports"))
            });
        }

        [HttpGet]
        public ActionResult ExecuteVerify()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ExecuteVerify(PreservationVerifyIndexModel postedModel)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                PreservationVerifyExecuteModel model = new PreservationVerifyExecuteModel
                {
                    fromDate = postedModel.fromDate,
                    toDate = postedModel.toDate
                };

                //crea l'elenco dei job di verifica 1 per conservazione chiusa
                List<PreservationVerifyJob> jobs = new List<PreservationVerifyJob>();
                foreach (Guid idArchive in postedModel.selectedArchives.Select(s => Guid.Parse(s)))
                {
                    //conservazioni chiuse per archivio
                    DocumentArchive archive = ArchiveService.GetArchive(idArchive);
                    IList<Preservation> preservations = _preservationService.ArchivePreservationClosedInDate(idArchive, postedModel.fromDate, postedModel.toDate.AddDays(1).AddSeconds(-1));
                    if (preservations.Count > 0)
                    {
                        jobs.AddRange(preservations.Select(p => new PreservationVerifyJob
                        {
                            idArchive = idArchive.ToString(),
                            idPreservation = p.IdPreservation.ToString(),
                            archiveName = archive.Name
                        }));
                    }
                    else
                    {
                        jobs.Add(new PreservationVerifyJob
                        {
                            idArchive = idArchive.ToString(),
                            idPreservation = Guid.Empty.ToString(),
                            archiveName = archive.Name
                        });
                    }
                }

                model.jobs = jobs.ToArray();
                return View(model);
            }, _loggerService);
        }        

        [NoAsyncTimeout]
        public async Task<ActionResult> DoVerify(PreservationVerifyExecuteModel model, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                return ActionResultHelper.TryCatchWithLogger(() =>
                {                    
                    ICollection<PreservationVerifyJob> jobResults = new List<PreservationVerifyJob>();
                    string cacheKey = $"{User.Identity.Name}_{model.executionId}";
                    if (!_cache_jobs.TryAdd(cacheKey, jobResults))
                    {
                        _loggerService.Error("E' avvenuto un errore nella fase di inizializzazione della cache per i risultati di verifica conservazione");
                        throw new Exception("Errore in inizializzazione cache risultati");
                    }

                    if (model == null || model.jobs == null || model.jobs.Length == 0)
                    {
                        _loggerService.Warn("Nessuna conservazione da verificare nel periodo indicato");
                        return Json(string.Empty);
                    }

                    ExecutePreservationVerifyInteractor interactor;
                    ExecutePreservationVerifyRequestModel requestModel;
                    foreach (PreservationVerifyJob job in model.jobs)
                    {
                        Preservation currentPreservation = _preservationService.GetPreservation(Guid.Parse(job.idPreservation), false);
                        if (currentPreservation == null)
                        {
                            jobResults.Add(new PreservationVerifyJob()
                            {
                                idArchive = job.idArchive,
                                archiveName = job.archiveName,
                                idPreservation = job.idPreservation.ToString(),
                                preservationLabel = "Nessuna conservazione da verificare nel periodo indicato",
                                verifyTitle = string.Empty,
                                result = "ok",
                                errors = string.Empty
                            });
                            _cache_jobs.AddOrUpdate(cacheKey, jobResults, (key, existingValue) => jobResults);
                            continue;
                        }

                        interactor = new ExecutePreservationVerifyInteractor(_loggerService);
                        requestModel = new ExecutePreservationVerifyRequestModel() { IdPreservation = Guid.Parse(job.idPreservation) };
                        ExecutePreservationVerifyResponseModel responseModel = interactor.Process(requestModel);
                        jobResults.Add(new PreservationVerifyJob
                        {
                            idArchive = job.idArchive,
                            archiveName = job.archiveName,
                            idPreservation = responseModel.IdPreservation.ToString(),
                            preservationLabel = $"Conservazione {currentPreservation.Label}",
                            verifyTitle = responseModel.VerifyTitle,
                            result = responseModel.Status == PreservationVerifyStatus.Ok ? "ok" : "bad",
                            errors = string.Join("<br />", responseModel.Errors)
                        });

                        _cache_jobs.AddOrUpdate(cacheKey, jobResults, (key, existingVal) => jobResults);
                    }

                    CreatePreservationVerifyReportInteractor reportInteractor = new CreatePreservationVerifyReportInteractor(_loggerService);
                    model.jobs = jobResults.ToArray();
                    PreservationVerifyReportRequestModel reportRequestModel = new PreservationVerifyReportRequestModel() { VerifyModel = model };
                    PreservationVerifyReportResponseModel reportResponseModel = reportInteractor.Process(reportRequestModel);
                    return Json(new
                    {
                        Jobs = jobResults,
                        Response = reportResponseModel
                    });
                }, _loggerService);                                  
            });
        }

        [HttpGet]
        public ActionResult DoVerifyProgress(Guid executionId)
        {
            if (!_cache_jobs.TryGetValue($"{User.Identity.Name}_{executionId}", out ICollection<PreservationVerifyJob> verifyJobs))
            {
                _loggerService.Warn("Errore nel recupero dei valori dalla cache dei risultati di verifica conservazione");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }            
            return Json(verifyJobs, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
