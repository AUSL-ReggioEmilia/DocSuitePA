using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Processes
{
    public static class ProcessFascicleTemplateFinder
    {
        public static bool NameAlreadyExists(this IRepository<ProcessFascicleTemplate> repository, string name, Guid idDossierFolder, bool optimization = true)
        {
            return repository.Queryable(optimization).Count(x => x.Name == name && x.DossierFolder.UniqueId == idDossierFolder) > 0;
        }

        public static IQueryable<ProcessFascicleTemplate> FindFascicleTemplatesByIdDossierFolder(this IRepository<ProcessFascicleTemplate> repository, Guid idDossierFolder, bool optimization = true)
        {
            return repository.Query(x => x.DossierFolder.UniqueId == idDossierFolder)
                .SelectAsQueryable();
        }
    }
}
