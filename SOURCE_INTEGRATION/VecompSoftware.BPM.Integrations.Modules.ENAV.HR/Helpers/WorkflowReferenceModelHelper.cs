using Newtonsoft.Json;
using System;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.HR.Configurations;
using VecompSoftware.DocSuiteWeb.Model.Workflow;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.HR.Helpers
{
    public static class WorkflowReferenceModelHelper
    {
        public static WorkflowReferenceModel CreateWorkflowReferenceModel<TModel>(TModel buildModel, Guid workflowReferenceId)
        {
            WorkflowReferenceModel dossierWorkflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = workflowReferenceId,
                ReferenceType = DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType.Build,
                ReferenceModel = JsonConvert.SerializeObject(buildModel, ModuleConfigurationHelper.JsonSerializerSettings)
            };

            return dossierWorkflowReferenceModel;
        }
    }
}
