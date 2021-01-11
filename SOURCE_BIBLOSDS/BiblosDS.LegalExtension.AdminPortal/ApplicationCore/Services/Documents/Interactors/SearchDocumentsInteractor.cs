using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Documents;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Services.Documents.Interactors
{
    public class SearchDocumentsInteractor : IInteractor<SearchDocumentsRequestModel, SearchDocumentsResponseModel>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public SearchDocumentsInteractor(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        public SearchDocumentsResponseModel Process(SearchDocumentsRequestModel request)
        {
            try
            {
                _logger.Info(string.Concat("SearchDocumentsInteractor -> search documents for archive ", request.IdArchive));
                BindingList<DocumentCondition> conditions = new BindingList<DocumentCondition>();
                Expression<Func<IQueryable<Library.Common.Model.Document>, object, IQueryable<Library.Common.Model.Document>>> queryExt = (x, y) => x.Where(xx => xx.IdParentBiblos != null 
                    && xx.PreservationDocuments.Any() && xx.IsLatestVersion == true && (!xx.IsDetached.HasValue || xx.IsDetached == false)).OrderBy(o => o.DateMain);

                if (request.FromDate.HasValue)
                {
                    Func<IQueryable<Library.Common.Model.Document>, object, IQueryable<Library.Common.Model.Document>> tmpCompile = queryExt.Compile();
                    queryExt = (x, y) => tmpCompile(x, y).Where(xx => xx.DateMain >= request.FromDate.Value);
                }

                if (request.ToDate.HasValue)
                {
                    Func<IQueryable<Library.Common.Model.Document>, object, IQueryable<Library.Common.Model.Document>> tmpCompile = queryExt.Compile();
                    queryExt = (x, y) => tmpCompile(x, y).Where(xx => xx.DateMain <= request.ToDate.Value);
                }

                if (request.DynamicFilters != null && request.DynamicFilters.Count > 0)
                {
                    foreach (KeyValuePair<string, string> filter in request.DynamicFilters)
                    {
                        if (!string.IsNullOrEmpty(filter.Value))
                        {
                            conditions.Add(new DocumentCondition()
                            {
                                Name = filter.Key,
                                Value = filter.Value,
                                Operator = Library.Common.Enums.DocumentConditionFilterOperator.Contains
                            });
                        }
                    }
                }

                ICollection<Document> foundDocuments = DocumentService.SearchDocumentsExt(request.IdArchive, conditions, queryExt.Compile(), null, out int counter, request.Skip, request.Top);
                return new SearchDocumentsResponseModel()
                {
                    Documents = foundDocuments,
                    Counter = counter
                };
            }
            catch (Exception ex)
            {
                _logger.Error(string.Concat("Process -> error on search documents ", ex.Message), ex);
                throw ex;
            }
        }
        #endregion        
    }
}