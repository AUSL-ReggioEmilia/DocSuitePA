using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.Documents;

namespace VecompSoftware.DocSuite.Document.DocumentProxy
{
	public interface IDocumentProxyContext
	{
		Task<ICollection<DocumentSuiteInfo>> InsertDocumentAsync(ArchiveDocumentProxy document);
		Task<ICollection<DocumentSuiteInfo>> GetInfoDocumentsAsync(Guid referenceId, int environment);
		Task<byte[]> GetDocumentContentAsync(Guid referenceId, string referenceType, Guid documentId);
	}
}
