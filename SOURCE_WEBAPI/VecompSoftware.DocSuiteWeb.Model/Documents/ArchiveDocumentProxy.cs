using System;

namespace VecompSoftware.DocSuiteWeb.Model.Documents
{
	public class ArchiveDocumentProxy
	{
		#region [Constructor]
		public ArchiveDocumentProxy()
		{

		}
		#endregion

		#region [Properties]
		public Guid ReferenceId { get; set; }

		public Guid DocumentId { get; set; }

		public string ReferenceType { get; set; }

		public string Name { get; set; }

		public int Environment { get; set; }

		public Guid StorageReferenceId { get; set; }

		public DocumentProxyCreateActionType CreateActionType { get; set; }

		public DocumentProxyStoreActionType StoreActionType { get; set; }

		public byte[] ContentStream { get; set; }

		#endregion
	}
}
