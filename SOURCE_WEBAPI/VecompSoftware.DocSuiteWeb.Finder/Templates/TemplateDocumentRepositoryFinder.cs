using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Templates
{
    public static class TemplateDocumentRepositoryFinder
    {
        public static ICollection<string> GetTags(this IRepository<TemplateDocumentRepository> repository)
        {
            return repository.Query(x => x.QualityTag != null && x.QualityTag != string.Empty, true).Select(s => s.QualityTag).AsEnumerable()
                .SelectMany(s => s.Split(';'))
                .GroupBy(g => new { QualityTag = g })
                .Select(s => s.Key.QualityTag)
                .ToList();
        }

        public static int CountNameAlreadyExist(this IRepository<TemplateDocumentRepository> repository, string name, Guid uniqueId)
        {
            return repository.Query(x => x.Name == name && x.UniqueId != uniqueId, true)
                .SelectAsQueryable()
                .Count();
        }

        public static Guid? GetDocumentIdArchiveChainById(this IRepository<TemplateDocumentRepository> repository, Guid idTemplate)
        {
            TemplateDocumentRepository query = repository.Query(x => x.UniqueId == idTemplate, optimization: true)
                .SelectAsQueryable()
                .FirstOrDefault();
            return query?.IdArchiveChain;
        }
    }
}
