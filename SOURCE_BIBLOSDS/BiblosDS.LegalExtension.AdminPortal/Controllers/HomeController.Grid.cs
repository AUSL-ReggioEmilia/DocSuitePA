using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using BiblosDS.LegalExtension.AdminPortal.Helpers;
using Kendo.Mvc.UI;
using BiblosDS.Library.Common.Utility;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using System.Web.Mvc;
using BiblosDS.Library.Common.Preservation.Services;
using BiblosDS.Library.Common.Preservation.IpdaDoc;
using BiblosDS.LegalExtension.AdminPortal.Models;
using System.ComponentModel;
using BiblosDS.LegalExtension.AdminPortal.ViewModel.Home;
using System.Linq.Expressions;
using Newtonsoft.Json;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Documents;
using BiblosDS.LegalExtension.AdminPortal.ViewModel;
using VecompSoftware.BiblosDS.WCF.Common;

namespace BiblosDS.LegalExtension.AdminPortal.Controllers
{
    public partial class HomeController
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]        
        [NoCache]
        public ActionResult GridArchives([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();
            try
            {
                int total = 0;
                int skip = request.Page - 1;
                int take = request.PageSize;

                skip = (skip < 1) ? 0 : skip * take;
                CustomerCompanyViewModel customerCompany = Session["idCompany"] as CustomerCompanyViewModel;

                if (WCFUtility.GetSettingValue("DBAdminLoginConnection") == "false")
                {
                    DocumentCondition conditions = new DocumentCondition();

                    List<DocumentSortCondition> sortConditions = new List<DocumentSortCondition>();
                    conditions.DocumentAttributeConditions = new System.ComponentModel.BindingList<DocumentCondition>();
                    conditions.DocumentAttributeConditions.Add(new DocumentCondition() { Name = "IsLegal", Value = 1, Operator = Library.Common.Enums.DocumentConditionFilterOperator.IsEqualTo, Condition = Library.Common.Enums.DocumentConditionFilterCondition.And });

                    if (request.Filters != null)
                    {
                        foreach (var item in request.Filters)
                        {
                            //conditions.DocumentAttributeConditions.Add(new DocumentCondition { Name = item., Value = item.Value, Operator = Library.Common.Enums.DocumentConditionFilterOperator.Contains, Condition = Library.Common.Enums.DocumentConditionFilterCondition.And });
                        }
                    }

                    if (request.Sorts != null)
                    {
                        foreach (var item in request.Sorts)
                        {
                            sortConditions.Add(new DocumentSortCondition { Name = item.Member, Dir = item.SortDirection });
                        }
                    }
                    else
                    {
                        sortConditions.Add(new DocumentSortCondition { Name = "Name", Dir = "ASC" });
                    }

                    
                    result.Data = ArchiveService.GetArchives(skip, take, conditions, sortConditions, out total, customerCompany.CompanyId).ToList();
                }
                else
                {
                    result.Data = UserArchive.GetUserArchivesPaged(User.Identity.Name, skip, take, out total, customerCompany.CompanyId);
                }

                result.Total = total;
            }
            catch (Exception ex)
            {
                result.Errors = ex;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult GridPreservations(Guid id, [DataSourceRequest] DataSourceRequest request)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                var result = new DataSourceResult();
                ICollection<Preservation> preservations = _preservationService.GetPreservations(id, request.PageSize, (request.Page - 1) * request.PageSize, true, false, out int counter);

                List<PreservationItem> gridItems = new List<PreservationItem>();
                foreach (var p in preservations)
                {
                    PreservationItem pi = new PreservationItem
                    {
                        IdPreservation = p.IdPreservation,
                        Label = p.Label,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        CloseDate = p.CloseDate,
                        Username = p.User.Name,
                        DisplayCreate = "none",
                        DisplaySign = "none",
                        DisplayClose = "none",
                        DisplayPurge = "none"
                    };

                    if (IpdaUtil.GetCloseFile(p.Path) != "" && IpdaUtil.GetIpdaXmlFile(p.Path) == "" && IpdaUtil.GetIpdaTsdFile(p.Path) == "")
                        pi.DisplayCreate = "inline";

                    if (IpdaUtil.GetIpdaXmlFile(p.Path) != "" && IpdaUtil.GetIpdaTsdFile(p.Path) == "" && !p.CloseDate.HasValue)
                        pi.DisplaySign = "inline";

                    if (IpdaUtil.GetIpdaXmlFile(p.Path) != "" && IpdaUtil.GetIpdaTsdFile(p.Path) != "" && !p.CloseDate.HasValue)
                        pi.DisplayClose = "inline";

                    if (p.CloseDate.HasValue && _preservationService.CountPreservationDocumentsToPurge(p.IdPreservation) > 0)
                    {
                        pi.DisplayPurge = "inline";
                    }

                    gridItems.Add(pi);
                }

                result.Total = counter;
                result.Data = gridItems;
                return Json(result, JsonRequestBehavior.AllowGet);
            }, _loggerService);
        }

        [NoCache]
        public ActionResult GridPreservationVerify([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();
            try
            {
                int total = 0;
                int skip = (request.Page - 1) * request.PageSize;
                int take = request.PageSize;

                if (skip < 0)
                    skip = 0;

                var service = new PreservationService();

                const int ARCHIVES_TAKE = 5;
                int archiveSkip = 0, totalArchives;

                List<DocumentArchive> archivi = null;
                CustomerCompanyViewModel customerCompany = Session["idCompany"] as CustomerCompanyViewModel;

                if (WCFUtility.GetSettingValue("DBAdminLoginConnection") == "false")
                {
                    DocumentCondition conditions = new DocumentCondition();
                    List<DocumentSortCondition> sortConditions = new List<DocumentSortCondition>();
                    conditions.DocumentAttributeConditions = new System.ComponentModel.BindingList<DocumentCondition>();
                    conditions.DocumentAttributeConditions.Add(new DocumentCondition() { Name = "IsLegal", Value = 1, Operator = Library.Common.Enums.DocumentConditionFilterOperator.IsEqualTo, Condition = Library.Common.Enums.DocumentConditionFilterCondition.And });
                    if (request.Filters != null)
                    {
                        foreach (var item in request.Filters)
                        {
                            //conditions.DocumentAttributeConditions.Add(new DocumentCondition { Name = item., Value = item.Value, Operator = Library.Common.Enums.DocumentConditionFilterOperator.Contains, Condition = Library.Common.Enums.DocumentConditionFilterCondition.And });
                        }
                    }
                    if (request.Sorts != null)
                    {
                        foreach (var item in request.Sorts)
                        {
                            sortConditions.Add(new DocumentSortCondition { Name = item.Member, Dir = item.SortDirection });
                        }
                    }
                    else
                    {
                        sortConditions.Add(new DocumentSortCondition { Name = "Name", Dir = "ASC" });
                    }

                    archivi = ArchiveService.GetArchives(skip, take, conditions, sortConditions, out total, customerCompany.CompanyId).ToList();
                }
                else
                {
                    archivi = UserArchive.GetUserArchivesPaged(User.Identity.Name, archiveSkip, ARCHIVES_TAKE, out totalArchives, customerCompany.CompanyId);

                    if (archivi.Count < totalArchives)
                    {
                        for (archiveSkip += ARCHIVES_TAKE; archiveSkip < totalArchives; archiveSkip += ARCHIVES_TAKE)
                        {
                            archivi.AddRange(UserArchive.GetUserArchivesPaged(User.Identity.Name, archiveSkip, ARCHIVES_TAKE, out totalArchives, customerCompany.CompanyId));
                        }
                    }

                    if (archivi.Any())
                        result.Data = service.GetPreservationVerify(archivi.Select(x => x.IdArchive).ToArray<Guid>(), skip, take, out total, null);
                    else
                        result.Data = archivi;
                }
                result.Total = total;
            }
            catch (Exception ex)
            {
                result.Errors = ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult ReadChildTasks(Guid id, [DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();
            try
            {
                var query = new PreservationService().GetAllChildPreservationTasks(id, (request.Page - 1) * request.PageSize, request.PageSize);

                result.Total = (int)query.TotalRecords;
                result.Data = query.Tasks;
            }
            catch (Exception ex)
            {
                result.Errors = ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult ReadTasks(Guid id, [DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();
            try
            {
                DocumentCondition conditions = new DocumentCondition();
                List<DocumentSortCondition> sortConditions = new List<DocumentSortCondition>();
                conditions.DocumentAttributeConditions = new System.ComponentModel.BindingList<DocumentCondition>();
                conditions.DocumentAttributeConditions.Add(new DocumentCondition() { Name = "IsLegal", Value = 1, Operator = Library.Common.Enums.DocumentConditionFilterOperator.IsEqualTo, Condition = Library.Common.Enums.DocumentConditionFilterCondition.And });
                if (request.Filters != null)
                {
                    foreach (var item in request.Filters)
                    {
                        //conditions.DocumentAttributeConditions.Add(new DocumentCondition { Name = item., Value = item.Value, Operator = Library.Common.Enums.DocumentConditionFilterOperator.Contains, Condition = Library.Common.Enums.DocumentConditionFilterCondition.And });
                    }
                }
                if (request.Sorts != null)
                {
                    foreach (var item in request.Sorts)
                    {
                        sortConditions.Add(new DocumentSortCondition { Name = item.Member, Dir = item.SortDirection });
                    }
                }
                else
                {
                    sortConditions.Add(new DocumentSortCondition { Name = "Name", Dir = "ASC" });
                }

                var query = new PreservationService().GetPreservationTasks(new System.ComponentModel.BindingList<DocumentArchive> { new DocumentArchive(id) }, (request.Page - 1) * request.PageSize, request.PageSize);

                result.Total = (int)query.TotalRecords;
                result.Data = query.Tasks;
            }
            catch (Exception ex)
            {
                result.Errors = ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        [Authorize]
        public ActionResult GetPreservationPendingTask([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = new DataSourceResult();

            CustomerCompanyViewModel customerCompany = Session["idCompany"] as CustomerCompanyViewModel;

            ICollection<PreservationTask> items = new List<PreservationTask>();
            try
            {
                BindingList<DocumentArchive> archives = null;
                if (WCFUtility.GetSettingValue("DBAdminLoginConnection") == "false")
                {
                    archives = new BindingList<DocumentArchive>(ArchiveService.GetLegalArchives("", customerCompany.CompanyId).Select(x => x.Archive).ToList());
                }
                else
                    archives = CustomerService.GetCustomerArchivesByUsername(User.Identity.Name);
                items = _preservationService.GetPreservationActiveTasks(archives.Select(s => s.IdArchive).ToList());
                result.Total = items.Count;
                result.Data = items;
            }
            catch (Exception ex)
            {
                result.Errors = ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

       

        [NoCache]
        [Authorize]
        public ActionResult GetPreservationVerifyTask([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = new DataSourceResult();
            CustomerCompanyViewModel customerCompany = Session["idCompany"] as CustomerCompanyViewModel;
            BindingList<DocumentArchive> archives = new BindingList<DocumentArchive>();
            try
            {
                if (WCFUtility.GetSettingValue("DBAdminLoginConnection") == "false")
                {
                    archives = CustomerService.GetCustomerArchivesByUsername(User.Identity.Name);
                }
                else
                    archives = new BindingList<DocumentArchive>(ArchiveService.GetLegalArchives("", customerCompany.CompanyId).Select(x => x.Archive).ToList());
                var tasks = new PreservationService().GetPreservationVerify(archives.Select(x => x.IdArchive).ToArray(), true);
                result.Total = tasks.Count;
                result.Data = tasks;
            }
            catch (Exception ex)
            {
                result.Errors = ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        [Authorize]
        public ActionResult FindDocuments([DataSourceRequest] DataSourceRequest request, Guid? idArchive, DateTime? fromDate, DateTime? toDate, string dynamicFilters)
        {
            return ActionResultHelper.TryCatchWithLogger(() =>
            {
                DataSourceResult result = new DataSourceResult();
                if (!idArchive.HasValue)
                {
                    result.Total = 0;
                    result.Data = new List<DistributionPackagesGridViewModel>();
                    return Json(result, JsonRequestBehavior.AllowGet);
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
                    DynamicFilters = filters,
                    Skip = (request.Page - 1) * request.PageSize,
                    Top = request.PageSize
                };
                SearchDocumentsResponseModel responseModel = _searchDocumentsInteractor.Process(requestModel);
                ICollection<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(idArchive.Value).OrderBy(x => x.Name).ToList();
                result.Total = responseModel.Counter;
                result.Data = responseModel.Documents.Select(s => new DistributionPackagesGridViewModel()
                {
                    IdDocument = s.IdDocument,
                    DocumentCreated = s.DateCreated,
                    DocumentName = s.Name,
                    IdPreservation = s.IdPreservation,
                    IsConservated = s.IdPreservation.HasValue,
                    Metadata = attributes.Select(ss => new DistributionPackagesGridMetadataViewModel()
                    {
                        MetadataName = ss.Name,
                        MetadataValue = FormatAttributeValue(s.AttributeValues.FirstOrDefault(x => x.Attribute.IdAttribute == ss.IdAttribute))
                    }).ToArray()
                });

                return Json(result, JsonRequestBehavior.AllowGet);
            }, _loggerService);
        }

        private string FormatAttributeValue(DocumentAttributeValue attributeValue)
        {
            if (attributeValue?.Value == null)
            {
                return string.Empty;
            }

            if (attributeValue.Attribute.AttributeType == "System.DateTime")
            {
                return DateTime.Parse(attributeValue.Value.ToString()).ToString("dd/MM/yyyy");
            }
            return attributeValue.Value.ToString();
        }


     

        #endregion
    }
}