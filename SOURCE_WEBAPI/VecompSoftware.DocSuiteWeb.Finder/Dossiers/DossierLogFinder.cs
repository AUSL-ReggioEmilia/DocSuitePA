using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Dossiers
{
    public static class DossierLogFinder
    {
        public static IQueryable<DossierLog> GetFolderHystoryLog(this IRepository<DossierLog> repository, Guid idDossierFolder)
        {
            return repository.Query(d => d.DossierFolder.UniqueId == idDossierFolder && d.LogType == DossierLogType.FolderHystory)
                .SelectAsQueryable();
        }

        public static IQueryable<DossierLog> GetLogsByHashValue(this IRepository<DossierLog> repository, string hashValue)
        {
            return repository.Query(d => d.Hash == hashValue)
                .SelectAsQueryable();
        }

    }
}
