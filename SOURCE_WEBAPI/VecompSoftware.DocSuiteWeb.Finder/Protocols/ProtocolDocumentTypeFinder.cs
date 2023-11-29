using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Protocols
{
    public static class ProtocolDocumentTypeFinder
    {
        public static IQueryable<ProtocolDocumentType> GetByCode(this IRepository<ProtocolDocumentType> repository, string code, bool optimization = true)
        {
            return repository.Query(x => x.Code == code, optimization)
                .SelectAsQueryable();
        }

        public static IQueryable<ProtocolDocumentType> GetByDescription(this IRepository<ProtocolDocumentType> repository, string description, bool optimization = true)
        {
            return repository.Query(x => x.Description == description, optimization)
                .SelectAsQueryable();
        }

        public static IQueryable<ProtocolDocumentType> GetByCodeOrDescription(this IRepository<ProtocolDocumentType> repository, string description, bool optimization = true)
        {
            return repository.Query(x => x.Code == description || x.Description == description, optimization)
                .SelectAsQueryable();
        }

        public static int CountDocumentTypeByCodeOrDescription(this IRepository<ProtocolDocumentType> repository, string description, bool optimization = false)
        {
            IQueryable<ProtocolDocumentType> partialQuery = repository.Query(x => x.Code == description || x.Description == description, optimization).SelectAsQueryable();
            return partialQuery.Count();
        }
    }
}
