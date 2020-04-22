using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors
{
    public interface IBaseCommonExecutor
    {
        #region [ Methods ]

        Task<DocumentUnit> Mapping(IContentBase value, IIdentityContext identityContext, bool isUpdate);
        IEvent CreateEvent(ICommandCQRS command, bool isUpdate, DocumentUnit documentUnit = null);
        Task CreateWorkflowActionsAsync(ICommandCQRS command, ICollection<IWorkflowAction> workflowActions);
        Task<DocumentUnit> SendDocumentAsync(DocumentUnit documentUnit, bool isUpdate);
        Task<bool> PushEventAsync(IEvent evt);

        #endregion

    }
}
