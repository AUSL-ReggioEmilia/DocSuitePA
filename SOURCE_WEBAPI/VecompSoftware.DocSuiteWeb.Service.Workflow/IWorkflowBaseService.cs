using VecompSoftware.DocSuiteWeb.Model.Workflow;

namespace VecompSoftware.DocSuiteWeb.Service.Workflow
{
    /// <summary>
    /// Interfaccia base contenente gli oggetti necessari per eseguire il wrapper del Workflow Client.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWorkflowBaseService<T> : IServiceAsync<T, WorkflowResult>
        where T : class
    {

    }
}
