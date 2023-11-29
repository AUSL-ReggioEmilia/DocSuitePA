using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Documents;

namespace VecompSoftware.DocSuite.Document.DocumentProxy
{
	[LogCategory(LogCategoryDefinition.DOCUMENTCONTEX)]
	public class DocumentProxyContext : IDocumentProxyContext
	{
		#region [ Fields ]
		private readonly ILogger _logger;
		private readonly string _grpcEndpoint;
		private readonly string _identityAccount;
		private readonly Guid _identityUniqueId;
		private readonly GrpcFactory _grpcFactory;
		private static ICollection<LogCategory> _logCategories;
		#endregion

		#region [ Properties ]
		protected static IEnumerable<LogCategory> LogCategories
		{
			get
			{
				if (_logCategories == null)
				{
					_logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(DocumentProxyContext));
				}
				return _logCategories;
			}
		}
		#endregion

		#region [ Constructor ]
		public DocumentProxyContext(
			ILogger logger,
			string grpcEndpoint,
			string identityAccount,
			Guid identityUniqueId,
			string pemCertificate)
        {
            _logger = logger;
			_grpcEndpoint = grpcEndpoint;
			_identityAccount = identityAccount;
			_identityUniqueId = identityUniqueId;

			if (string.IsNullOrWhiteSpace(_grpcEndpoint))
			{
				throw new DSWException($"L'endpoint gRPC di DocumentProxy non è stato definito o il suo valore non è corretto.", null, DSWExceptionCode.SS_RulesetValidation);
			}

			_grpcFactory = new GrpcFactory(_logger, _grpcEndpoint, pemCertificate);
		}
		#endregion

		#region [ Methods ]
		private Identity InitializeIdentity()
		{
			if (string.IsNullOrWhiteSpace(_identityAccount) || _identityUniqueId == Guid.Empty)
			{
				throw new DSWException($"Le informazioni relative alla identity per la chiamata gRPC al DocumentProxy non sono state definite o il loro valore non è corretto.", null, DSWExceptionCode.SS_RulesetValidation);
			}
			return new Identity()
			{
				Account = _identityAccount,
				UniqueId = _identityUniqueId.ToString()
			};
		}

		public async Task<ICollection<DocumentSuiteInfo>> GetInfoDocumentsAsync(Guid referenceId, int environment)
		{
			Identity identity = InitializeIdentity();
			GetReferenceInfoReply reply = await _grpcFactory.GetDocumentProxyClient().GetReferenceInfoAsync(new GetReferenceInfoRequest()
			{
				ReferenceId = referenceId.ToString(),
				Environment = environment,
				Identity = identity
			});

			if (reply.Status != MessageStatus.Success)
			{
				throw new DSWException($"Il documentproxy non ha trovato la corrispondenza dei dati referenceId:{referenceId} environment:{environment}. Verificare se il documento è presente nella suite", null, DSWExceptionCode.DM_Anomaly);
			}

			return reply.DocumentInfos.Select(f => new DocumentSuiteInfo(
				f.ReferenceId,
				f.ReferenceType,
				f.FileExtension,
				f.Size,
				f.DocumentId,
				f.FileName,
				f.VirtualPath
			)).ToList();
		}

		public async Task<byte[]> GetDocumentContentAsync(Guid referenceId, string referenceType, Guid documentId)
		{
			Identity identity = InitializeIdentity();
			GetDocumentReply reply = await _grpcFactory.GetDocumentProxyClient().GetDocumentAsync(new GetDocumentRequest()
			{
				DocumentId = documentId.ToString(),
				ReferenceId = referenceId.ToString(),
				ReferenceType = referenceType,
				Identity = identity
			});

			if (reply.Status != MessageStatus.Success)
			{
				throw new DSWException($"Il documentproxy non ha trovato la corrispondenza dei dati referenceId:{referenceId} documentId:{documentId} referenceType:{referenceType}. Verificare se il documento è presente nella suite", null, DSWExceptionCode.DM_Anomaly);
			}

			return reply.Document.Document.Content.ToByteArray();
		}

		public async Task<ICollection<DocumentSuiteInfo>> InsertDocumentAsync(ArchiveDocumentProxy document)
		{			
			Identity identity = InitializeIdentity();
			CreateDocumentReply reply = await _grpcFactory.GetDocumentProxyClient().SaveAsync(new CreateDocumentRequest()
			{
				FileName = document.Name,
				Content = ByteString.CopyFrom(document.ContentStream),
				ReferenceId = document.ReferenceId.ToString(),
				ReferenceType = document.ReferenceType,
				ActionType = (CreateActionType)document.CreateActionType,
				DocumentId = document.DocumentId.ToString(),
				Environment = document.Environment,
				StorageReferenceId = document.StorageReferenceId.ToString(),
				StoreActionType = (StoreActionType)document.StoreActionType,
				Identity = identity
			});

			if (reply.Status != MessageStatus.Success)
			{
				throw new DSWException($"An error occured when trying to upload document '{document.Name}' with id '{document.DocumentId}' [ErrorCode: {reply.Error.ErrorCode}]: {reply.Error.Details}", null, DSWExceptionCode.DM_Anomaly);
			}

			GetReferenceInfoReply replyGetInfo = _grpcFactory.GetDocumentProxyClient().GetReferenceInfo(new GetReferenceInfoRequest()
			{
				ReferenceId = reply.ReferenceId,
				Environment = document.Environment,
				Identity = identity
			});

			if (replyGetInfo == null || replyGetInfo.Status != MessageStatus.Success)
			{
				throw new DSWException($"Il documentproxy non ha trovato la corrispondenza dei dati referenceId:{document.ReferenceId} documentId:{document.DocumentId} environment:{document.Environment}. Verificare se il documento è presente nella suite", null, DSWExceptionCode.DM_Anomaly);
			}

			return replyGetInfo.DocumentInfos.Select(f => new DocumentSuiteInfo(
				f.ReferenceId,
				f.ReferenceType,
				f.FileExtension,
				f.Size,
				f.DocumentId,
				f.FileName,
				f.VirtualPath,
				f.DocumentId == document.DocumentId.ToString()
			)).ToList();
		} 
		#endregion
	}
}
