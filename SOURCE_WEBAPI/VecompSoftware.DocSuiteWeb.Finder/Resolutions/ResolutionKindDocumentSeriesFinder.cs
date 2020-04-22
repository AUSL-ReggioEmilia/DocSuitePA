using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Resolutions
{
    public static class ResolutionKindDocumentSeriesFinder
    {
        public static bool HasAlreadyDocumentSeries(this IRepository<ResolutionKindDocumentSeries> repository, int idDocumentSeries, Guid idResolutionKind, Guid idResolutionKindDocumentSeries, bool optimization = true)
        {
            return repository.Queryable(optimization)
                .Count(x => x.DocumentSeries.EntityId == idDocumentSeries && x.ResolutionKind.UniqueId == idResolutionKind && x.UniqueId != idResolutionKindDocumentSeries) > 0;
        }
    }
}
