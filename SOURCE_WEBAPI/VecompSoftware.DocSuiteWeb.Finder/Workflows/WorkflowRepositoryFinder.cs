using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Workflows
{
    public static class WorkflowRepositoryFinder
    {
        public static WorkflowRepository GetIncludingEvaluationProperties(this IRepository<WorkflowRepository> repository, Guid workflowRepositoryId)
        {
            return repository
                .Query(f => f.UniqueId == workflowRepositoryId, true)
                .Include(p => p.WorkflowEvaluationProperties)
                .Select().FirstOrDefault();
        }

        public static WorkflowRepository GetByInstanceId(this IRepository<WorkflowRepository> repository, Guid workflowInstanceId)
        {
            return repository.Query(f => f.WorkflowInstances.Any(x => x.UniqueId == workflowInstanceId), true)
                            .Select()
                            .FirstOrDefault();
        }

        public static WorkflowRepository GetByName(this IRepository<WorkflowRepository> repository, string workflowName, bool optimization = false)
        {
            return repository.Query(f => f.Name == workflowName, optimization: optimization)
                            .Select()
                            .FirstOrDefault();
        }

        public static WorkflowRepository GetPublicWorkflowByName(this IRepository<WorkflowRepository> repository, string workflowName, bool optimization = false)
        {
            return repository.Query(f => f.Name == workflowName && f.DSWEnvironment == DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Any, optimization: optimization)
                    .Include(f => f.WorkflowRoleMappings)
                    .Include(f => f.WorkflowEvaluationProperties)
                    .Select().FirstOrDefault();
        }

        public static ICollection<WorkflowRepository> GetAuthorizedActiveWorkflowRepositories(this IRepository<WorkflowRepository> repository, string username, string domain, int env, bool anyEnv, 
            bool documentRequired, bool showOnlyNoInstanceWorkflows, bool showOnlyHasIsFascicleClosedRequired, bool documentUnitRequired)
        {
            return repository.ExecuteModelFunction<WorkflowRepository>(CommonDefinition.SQL_FX_WorkflowRepository_AuthorizedActiveWorkflowRepositories,
                new QueryParameter(CommonDefinition.SQL_Param_WorkflowRepository_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_WorkflowRepository_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_WorkflowRepository_Environment, env),
                new QueryParameter(CommonDefinition.SQL_Param_WorkflowRepository_AnyEnvironment, anyEnv),
                new QueryParameter(CommonDefinition.SQL_Param_WorkflowRepository_DocumentRequired, documentRequired),
                new QueryParameter(CommonDefinition.SQL_Param_WorkflowRepository_DocumentUnitRequired, documentUnitRequired),
                new QueryParameter(CommonDefinition.SQL_Param_WorkflowRepository_ShowOnlyNoInstanceWorkflows, showOnlyNoInstanceWorkflows),
                new QueryParameter(CommonDefinition.SQL_Param_WorkflowRepository_ShowOnlyHasIsFascicleClosedRequired, showOnlyHasIsFascicleClosedRequired));

        }

        public static bool HasAuthorizedWorkflowRepositories(this IRepository<WorkflowRepository> repository, string username, string domain, int env, bool anyEnv)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_WorkflowRepository_HasAuthorizedWorkflowRepositories,
                new QueryParameter(CommonDefinition.SQL_Param_WorkflowRepository_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_WorkflowRepository_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_WorkflowRepository_Environment, env),
                new QueryParameter(CommonDefinition.SQL_Param_WorkflowRepository_AnyEnvironment, anyEnv));
        }

        public static IQueryable<WorkflowRepository> GetByWorkflowActivityId(this IRepository<WorkflowRepository> repository, Guid workflowActivityId)
        {
            return repository.Query(f => f.WorkflowInstances.Any(x => x.WorkflowActivities.Any(y => y.UniqueId == workflowActivityId)), true)
                            .SelectAsQueryable();
        }
    }
}
