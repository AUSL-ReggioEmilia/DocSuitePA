using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Workflows
{
    public static class WorkflowRoleMappingFinder
    {
        public static IEnumerable<WorkflowRoleMapping> GetByMappingTag(this IRepository<WorkflowRoleMapping> repository, string mappingTag, Guid workflowRepositoryId)
        {
            return repository.Query(f => f.MappingTag == mappingTag && f.WorkflowRepository.UniqueId == workflowRepositoryId, true)
                            .Include(f => f.Role)
                            .Select();
        }

        public static IEnumerable<WorkflowRoleMapping> GetByMappingTag(this IRepository<WorkflowRoleMapping> repository, string mappingTag, Guid workflowRepositoryId, string activityId)
        {
            return repository.Query(f => f.MappingTag == mappingTag && f.WorkflowRepository.UniqueId == workflowRepositoryId && f.IdInternalActivity == activityId, true)
                            .Include(f => f.Role)
                            .Select();
        }
    }
}
