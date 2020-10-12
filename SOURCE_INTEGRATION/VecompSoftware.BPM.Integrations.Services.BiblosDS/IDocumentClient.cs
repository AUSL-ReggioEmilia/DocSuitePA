using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService;

namespace VecompSoftware.BPM.Integrations.Services.BiblosDS
{
    public interface IDocumentClient
    {
        string ATTRIBUTE_SIGNATURE { get; }

        Task<ICollection<ArchiveDocument>> InsertDocumentsAsync(ICollection<ArchiveDocument> documentModels);

        Task<ArchiveDocument> InsertDocumentAsync(ArchiveDocument documentModel);

        Task<byte[]> GetDocumentStreamAsync(Guid chainId);

        Task DetachDocumentAsync(Guid documentId);

        Task<Guid> UpdateChainAsync(string archiveName, Guid documentId, IDictionary<string, object> metadatas);

        Task<Guid> UpdateDocumentAsync(string archiveName, Guid documentId, IDictionary<string, object> metadatas);

        Task<ICollection<ArchiveDocument>> FindDocumentsAsync(ICollection<FinderCondition> conditions);

        Task<ArchiveDocument> AddDocumentToChainAsync(Guid idChain, ArchiveDocument documentModel);

        Task<bool> IsSignedAsync(byte[] source);

        Task AddDocumentToDocumentUnitAsync(ArchiveDocument document, Guid idDocumentUnit);

        Task<ArchiveDocument> GetInfoDocumentAsync(Guid documentId);

        Task<ICollection<ArchiveDocument>> GetChildrenAsync(Guid parentId);

        Task RemoveFullTextDataAsync(Guid chainId);

        Task AlignFullTextDataAsync(Guid chainId);

        Task<List<Document>> GetDocumentChildrenAsync(Guid idChain);

        Task<Content> GetDocumentContentByIdAsync(Guid idDocument);

        Task<Document> AddDocumentToChainAsync(Document document);

        Task<IList<Archive>> GetArchivesAsync();

        Task<IList<DocumentService.Attribute>> GetAttributesDefinitionAsync(string archiveName);

        Task<IList<DocumentSignInfo>> GetDocumentSignInfoAsync(Guid idDocument);
    }
}