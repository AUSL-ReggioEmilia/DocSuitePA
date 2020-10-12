using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Archives;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Services.Archives.Interactors;
using BiblosDS.LegalExtension.AdminPortal.Helpers;
using BiblosDS.LegalExtension.AdminPortal.Infrastructure.Services.Common;
using BiblosDS.LegalExtension.AdminPortal.ViewModel;
using BiblosDS.LegalExtension.AdminPortal.ViewModel.Archives;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace BiblosDS.LegalExtension.AdminPortal.Controllers
{
    [Authorize]
    public class ArchiveController : Controller
    {
        #region [ Fields ]
        private readonly ILog _logger = LogManager.GetLogger(typeof(ArchiveController));
        private readonly ILogger _loggerService;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ArchiveController()
        {
            _loggerService = new LoggerService(_logger);
        }
        #endregion

        #region [ Methods ]
        [NoCache]
        public ActionResult PreservationArchivesConfigurable()
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                return View();
            }, _loggerService);
        }

        [NoCache]
        public ActionResult GetPreservationArchivesConfigurable([DataSourceRequest] DataSourceRequest request)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                ICollection<ArchiveConfigurableViewModel> results = new List<ArchiveConfigurableViewModel>();
                CustomerCompanyViewModel customerCompany = Session["idCompany"] as CustomerCompanyViewModel;
                string archiveFilterName = string.Empty;
                if (request.Filters.Count > 0)
                {
                    archiveFilterName = (request.Filters.First() as FilterDescriptor).Value.ToString();
                }
                ICollection<DocumentArchive> archives = ArchiveService.GetPreservationArchivesConfigurable(customerCompany.CompanyId, archiveFilterName, (request.Page - 1) * request.PageSize, request.PageSize, out int totalItems);
                foreach (DocumentArchive archive in archives)
                {
                    results.Add(new ArchiveConfigurableViewModel()
                    {
                        IdArchive = archive.IdArchive,
                        IsPreservationEnabled = archive.IsLegal,
                        ArchiveName = archive.Name
                    });
                }

                DataSourceResult result = new DataSourceResult()
                {
                    Total = totalItems,
                    Data = results
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }, _loggerService);
        }

        [NoCache]
        public ActionResult ArchiveConfigurationWizard(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                DocumentArchive archive = ArchiveService.GetArchive(id);
                if (archive == null)
                {
                    throw new Exception(string.Format("Nessun archivio trovato con id {0}", id));
                }

                ArchiveConfigurationWizardViewModel model = new ArchiveConfigurationWizardViewModel()
                {
                    IdArchive = id
                };
                return View(model);
            }, _loggerService);
        }

        [HttpPost]
        public ActionResult ArchiveConfigurationWizard(ArchiveWizardStepBaseViewModel model)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                Func<ArchiveWizardStepBaseViewModel, ActionResult> func = null;
                switch (model.ActiveStep)
                {
                    case 1:
                        {
                            ModelState.Remove(nameof(ArchiveWizardStepBaseViewModel.SelectedPreservationAttributes));
                            func = (m) => ConfirmAndNextArchivePreservationConfiguration(m);
                        }
                        break;
                    case 2:
                        {
                            ModelState.Remove(nameof(ArchiveWizardStepBaseViewModel.MainDateAttribute));
                            ModelState.Remove(nameof(ArchiveWizardStepBaseViewModel.PathPreservation));
                            ModelState.Remove(nameof(ArchiveWizardStepBaseViewModel.SelectedPrimaryKeyAttributes));
                            func = (m) => ConfirmAndNextArchivePreservationAttributesConfiguration(m);
                        }
                        break;
                    case 3:
                        {
                            ModelState.Clear();
                            ArchiveConfigurationWizardViewModel returnModel = new ArchiveConfigurationWizardViewModel()
                            {
                                IdArchive = model.IdArchive,
                                FlowActiveIndex = 4,
                                IsCompleteWithErrors = model.IsCompleteWithErrors.Value
                            };
                            return View(returnModel);
                        }
                    case 4:
                        {
                            return RedirectToAction("PreservationArchivesConfigurable", "Archive");
                        }
                    default:
                        throw new Exception($"Step {model.ActiveStep} not valid");
                }

                if (!ModelState.IsValid)
                {
                    ArchiveConfigurationWizardViewModel resultModel = new ArchiveConfigurationWizardViewModel()
                    {
                        IdArchive = model.IdArchive,
                        FlowActiveIndex = model.ActiveStep
                    };
                    return View(resultModel);
                }

                return func(model);
            }, _loggerService);
        }

        [NoCache]
        public ActionResult CheckArchivePreservationConfiguration(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                CheckPreservationArchiveConfigurationViewModel model = new CheckPreservationArchiveConfigurationViewModel()
                {
                    IdArchive = id
                };
                DocumentArchive archive = ArchiveService.GetArchive(id);
                ICollection<DocumentAttribute> archiveAttributes = AttributeService.GetAttributesFromArchive(id);
                model.ArchiveName = archive.Name;
                model.ArchiveAttributes = archiveAttributes.Where(x => !x.KeyOrder.HasValue || x.KeyOrder == 0)
                                                .OrderBy(o => o.DisplayName)
                                                .ToDictionary(k => k.IdAttribute.ToString(), v => v.DisplayName);                                                
                //model.DateAttributes.Add(Guid.Empty.ToString(), "--Crea attributo (Date)");
                archiveAttributes.Where(x => x.AttributeType.Equals("System.DateTime", StringComparison.InvariantCultureIgnoreCase))
                    .OrderBy(o => o.DisplayName).ToList().ForEach(f => model.DateAttributes.Add(f.IdAttribute.ToString(), f.DisplayName));
                model.PathPreservation = archive.PathPreservation;
                model.MainDateAttribute = archiveAttributes.Where(x => x.IsMainDate == true).Select(s => s.IdAttribute.ToString()).SingleOrDefault();
                model.PrimaryKeyAttributes = archiveAttributes.Where(x => x.KeyOrder > 0).OrderBy(o => o.KeyOrder)
                                                .ToDictionary(k => k.IdAttribute.ToString(), v => v.DisplayName);

                ValidateArchiveForPreservationInteractor interactor = new ValidateArchiveForPreservationInteractor(_loggerService);
                ValidateArchiveForPreservationResponseModel response = interactor.Process(new ValidateArchiveForPreservationRequestModel() { IdArchive = id });

                model.HasPreservations = response.HasPreservations;
                model.ValidationErrors = response.ValidationErrors;
                model.IsValidated = response.IsValidated;

                return PartialView("_CheckArchivePreservationConfiguration", model);
            }, _loggerService);
        }

        private ActionResult ConfirmAndNextArchivePreservationConfiguration(ArchiveWizardStepBaseViewModel model)
        {
            short i = 1;
            IDictionary<Guid, short> orderedPrimaryKeyAttributes = new Dictionary<Guid, short>();
            foreach (string primaryKeyAttribute in model.SelectedPrimaryKeyAttributes)
            {
                orderedPrimaryKeyAttributes.Add(Guid.Parse(primaryKeyAttribute), i);
                i++;
            }

            ModifyArchivePreservationConfigurationRequestModel requestModel = new ModifyArchivePreservationConfigurationRequestModel()
            {
                IdArchive = model.IdArchive,
                PathPreservation = model.PathPreservation,
                MainDateAttribute = Guid.Parse(model.MainDateAttribute),
                OrderedPrimaryKeyAttributes = orderedPrimaryKeyAttributes
            };
            ModifyArchivePreservationConfigurationInteractor interactor = new ModifyArchivePreservationConfigurationInteractor(_loggerService);
            interactor.Process(requestModel);

            ArchiveConfigurationWizardViewModel returnModel = new ArchiveConfigurationWizardViewModel()
            {
                IdArchive = model.IdArchive,
                FlowActiveIndex = 2
            };
            return View("ArchiveConfigurationWizard", returnModel);
        }

        [NoCache]
        public ActionResult ArchivePreservationAttributesConfiguration(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                DocumentArchive archive = ArchiveService.GetArchive(id);
                ICollection<DocumentAttribute> archiveAttributes = AttributeService.GetAttributesFromArchive(id);
                ArchivePreservationAttributesConfigurationViewModel model = new ArchivePreservationAttributesConfigurationViewModel()
                {
                    IdArchive = id,
                    ArchiveName = archive.Name,
                    ArchiveAttributes = archiveAttributes.Where(x => !x.ConservationPosition.HasValue || x.ConservationPosition == 0)
                                            .OrderBy(o => o.DisplayName)
                                            .ToDictionary(k => k.IdAttribute.ToString(), v => v.DisplayName),
                    PreservationAttributes = archiveAttributes.Where(x => x.ConservationPosition > 0).OrderBy(o => o.ConservationPosition)
                                            .ToDictionary(k => k.IdAttribute.ToString(), v => v.DisplayName)
                };

                return PartialView("_ArchivePreservationAttributesConfiguration", model);
            }, _loggerService);
        }

        private ActionResult ConfirmAndNextArchivePreservationAttributesConfiguration(ArchiveWizardStepBaseViewModel model)
        {
            short i = 1;
            IDictionary<Guid, short> orderedPreservationAttributes = new Dictionary<Guid, short>();
            foreach (string preservationAttribute in model.SelectedPreservationAttributes)
            {
                orderedPreservationAttributes.Add(Guid.Parse(preservationAttribute), i);
                i++;
            }

            ModifyArchivePreservationAttributesRequestModel requestModel = new ModifyArchivePreservationAttributesRequestModel()
            {
                IdArchive = model.IdArchive,
                OrderedPreservationAttributes = orderedPreservationAttributes
            };
            ModifyArchivePreservationAttributesInteractor interactor = new ModifyArchivePreservationAttributesInteractor(_loggerService);
            interactor.Process(requestModel);

            ArchiveConfigurationWizardViewModel returnModel = new ArchiveConfigurationWizardViewModel()
            {
                IdArchive = model.IdArchive,
                FlowActiveIndex = 3
            };
            return View("ArchiveConfigurationWizard", returnModel);
        }

        [NoCache]
        public ActionResult ExecuteArchivePreservationMigrator(Guid id)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                DocumentArchive archive = ArchiveService.GetArchive(id);
                CustomerCompanyViewModel customerCompany = Session["idCompany"] as CustomerCompanyViewModel;
                ExecuteArchivePreservationMigratorViewModel model = new ExecuteArchivePreservationMigratorViewModel()
                {
                    IdArchive = id,
                    ArchiveName = archive.Name,
                    IdCompany = customerCompany.CompanyId
                };
                return PartialView("_ExecuteArchivePreservationMigrator", model);
            }, _loggerService);
        }

        [NoCache]
        public ActionResult ArchivePreservationConfigurationSummary(Guid id, bool isCompleteWithErrors)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                DocumentArchive archive = ArchiveService.GetArchive(id);
                ICollection<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(id);
                ArchiveWizardStepBaseViewModel model = new ArchiveWizardStepBaseViewModel()
                {
                    IdArchive = id,
                    ArchiveName = archive.Name,
                    PathPreservation = archive.PathPreservation,
                    MainDateAttribute = attributes.SingleOrDefault(s => s.IsMainDate == true).DisplayName,
                    SelectedPreservationAttributes = attributes.Where(x => x.ConservationPosition > 0).OrderBy(o => o.ConservationPosition).Select(s => s.DisplayName).ToList(),
                    SelectedPrimaryKeyAttributes = attributes.Where(x => x.KeyOrder > 0).OrderBy(o => o.KeyOrder).Select(s => s.DisplayName).ToList(),
                    IsCompleted = true,
                    IsCompleteWithErrors = isCompleteWithErrors
                };
                return PartialView("_ArchivePreservationConfigurationSummary", model);
            }, _loggerService);
        }
        #endregion
    }
}