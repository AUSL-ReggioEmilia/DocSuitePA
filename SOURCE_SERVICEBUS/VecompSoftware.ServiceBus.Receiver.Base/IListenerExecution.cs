using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.Services.Command.CQRS.Commands;

namespace VecompSoftware.ServiceBus.Receiver.Base
{
    public interface IListenerExecution<TCommand>
        where TCommand : ICommand
    {
        #region [ Property ]
        IDictionary<string, object> Properties { get; set; }

        EvaluationModel RetryPolicyEvaluation { get; set; }
        #endregion

        #region [ Methods ]
        Task ExecuteAsync(TCommand entity);
        #endregion
    }
}
