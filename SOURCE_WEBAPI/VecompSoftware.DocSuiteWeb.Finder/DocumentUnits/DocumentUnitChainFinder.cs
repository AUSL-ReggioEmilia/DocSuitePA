using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.DocumentUnits
{
    public static class DocumentUnitChainFinder
    {
        public static IQueryable<DocumentUnitChain> GetByDocumentUnit(this IRepository<DocumentUnitChain> repository, DocumentUnit documentUnit, bool optimization = false)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == documentUnit.UniqueId, optimization)
                .Include(i => i.DocumentUnit)
                .SelectAsQueryable();
        }

        public static DocumentUnitChain GetByDocumentUnitAndChainType(this IRepository<DocumentUnitChain> repository, DocumentUnit documentUnit, ChainType chainType, bool optimization = false)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == documentUnit.UniqueId && x.ChainType == chainType, optimization)
                .Include(i => i.DocumentUnit)
                .SelectAsQueryable()
                .SingleOrDefault();
        }
    }
}
