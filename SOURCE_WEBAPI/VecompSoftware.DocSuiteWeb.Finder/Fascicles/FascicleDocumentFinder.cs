using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Fascicles
{
    public static class FascicleDocumentFinder
    {
        public static IQueryable<FascicleDocument> GetByIdFascicleFolder(this IRepository<FascicleDocument> repository, Guid idFascicleFolder, bool optimization = false, bool includ**REMOVE**igationProperties = true)
        {
            if (includ**REMOVE**igationProperties)
            {
                return repository.Query(x => x.FascicleFolder.UniqueId == idFascicleFolder, optimization: optimization)
                    .Include(i => i.FascicleFolder)
                    .SelectAsQueryable();
            }
            return repository.Query(x => x.FascicleFolder.UniqueId == idFascicleFolder, optimization: optimization)
                .SelectAsQueryable();
        }

        public static IQueryable<FascicleDocument> GetByFascicle(this IRepository<FascicleDocument> repository, Guid idFascicle, bool optimization = false)
        {
            return repository.Query(x => x.Fascicle.UniqueId == idFascicle, optimization: optimization)
                .SelectAsQueryable();
        }

        public static IQueryable<FascicleDocument> GetByFascicleFolderId(this IRepository<FascicleDocument> repository, Guid idFascicleFolder, bool optimization = false)
        {
            return repository.Query(x => x.FascicleFolder.UniqueId == idFascicleFolder, optimization: optimization)
                .SelectAsQueryable();
        }

        public static bool HasFascicleDocument(this IRepository<FascicleDocument> repository, string folderName, Guid idFascicle)
        {
            bool res = repository.Queryable(true).Any(x => x.FascicleFolder.Name == folderName && x.Fascicle.UniqueId == idFascicle);
            return res;
        }

        public static IQueryable<FascicleDocument> HasFascicleDocumentSigned(this IRepository<FascicleDocument> repository, string folderName, Guid idFascicle, bool optimization = false)
        {
            
            return repository.Query(x => x.FascicleFolder.Name == folderName && x.Fascicle.UniqueId == idFascicle, optimization: optimization).SelectAsQueryable();
        }
    }
}