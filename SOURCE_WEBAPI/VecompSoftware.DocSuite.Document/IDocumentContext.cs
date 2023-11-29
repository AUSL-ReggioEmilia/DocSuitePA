using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuite.Document
{
    public interface IDocumentContext<TContext, TArchiveDocument> : IDisposable
        where TContext : class, new()
    {
        Task<ICollection<TContext>> GetDocumentsFromChainAsync(Guid idChain);

        Task<TArchiveDocument> InsertDocumentAsync(TArchiveDocument documentModel);

        Task<ICollection<TArchiveDocument>> InsertDocumentsAsync(ICollection<TArchiveDocument> documents, Guid? idChain = null);

        Task<TArchiveDocument> UpdateDocumentAsync(TArchiveDocument documentModel, Dictionary<string, string> attributes);

        Task<IEnumerable<TContext>> GetDocumentLatestVersionFromChainAsync(Guid idChain);

        Task<byte[]> GetDocumentContentAsync(Guid idDocument);

        Task<TContext> GetDocumentAsync(Guid idDocument);

        Task<bool> HasActiveDocumentsAsync(Guid idChain);

        bool HasActiveDocuments(Guid idChain);
        Task<bool> IsDocumentsSignedAsync(List<Guid> idDocuments);

        Task<ICollection<Guid>> FullTextFindDocumentsAsync(IList<string> archiveNames, string filter);
        
        Task<Guid> GetDocumentIdAsync(int idBiblos, string archiveName);

        Task DetachDocumentAsync(Guid idDocument);
    }
}