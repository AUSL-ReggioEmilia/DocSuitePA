using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Fascicles
{
    /// <summary>
    /// Extending IRepository<FascicleLink>
    /// </summary>
    public static class FascicleLinkFinder
    {
        public static IQueryable<FascicleLink> GetByFascicleLink(this IRepository<FascicleLink> repository, Fascicle fascicle, Fascicle fascicleLinked)
        {
            IQueryable<FascicleLink> temp = repository.Query(x => x.Fascicle.UniqueId == fascicle.UniqueId && x.FascicleLinked.UniqueId == fascicleLinked.UniqueId)
                .Include(i => i.Fascicle)
                .Include(i => i.FascicleLinked)
                .SelectAsQueryable();
            return temp;
        }
    }
}
