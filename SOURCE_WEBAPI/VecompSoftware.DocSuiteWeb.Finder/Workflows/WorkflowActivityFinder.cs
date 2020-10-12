using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.Helpers.Workflow;

namespace VecompSoftware.DocSuiteWeb.Finder.Workflows
{
    public static class WorkflowActivityFinder
    {
        public static IEnumerable<WorkflowActivity> Get(this IRepository<WorkflowActivity> repository, Guid workflowActivityId)
        {
            return repository.Query(f => f.UniqueId == workflowActivityId)
                .Include(f => f.WorkflowInstance)
                .Include(f => f.WorkflowInstance.WorkflowProperties)
                .Include(f => f.WorkflowInstance.WorkflowRepository)
                .Include(f => f.WorkflowProperties)
                .Select();
        }

        public static bool HasHandler(this IRepository<WorkflowActivity> repository, Guid workflowActivityId)
        {
            return repository.Queryable(true).Count(x => x.UniqueId == workflowActivityId && x.WorkflowAuthorizations.Any(w => w.IsHandler) && x.Status < WorkflowStatus.Done) > 0;
        }

        public static bool IsWorkflowActivityHandler(this IRepository<WorkflowActivity> repository, string username, string domain, Guid workflowActivityId)
        {
            string account = string.Concat(domain, "\\", username);
            return repository.Queryable(true).Count(x => x.UniqueId == workflowActivityId && x.WorkflowAuthorizations.Any(a => a.IsHandler && a.Account == account)) > 0;
        }

        public static bool IsAuthorized(this IRepository<WorkflowActivity> repository, string username, string domain, Guid workflowActivityId)
        {
            string account = string.Concat(domain, "\\", username);
            return repository.Queryable(true).Count(x => x.UniqueId == workflowActivityId && x.WorkflowAuthorizations.Any(a => a.Account == account)) > 0;
        }

        public static WorkflowActivity GetByUniqueId(this IRepository<WorkflowActivity> repository, Guid workflowActivityId, bool optimization = true)
        {
            return repository.Query(x => x.UniqueId == workflowActivityId, optimization: optimization).Include(x => x.WorkflowAuthorizations).SelectAsQueryable().SingleOrDefault();
        }

        public static IQueryable<WorkflowActivity> GetActiveActivitiesByReferenceIdAndEnvironment(this IRepository<WorkflowActivity> repository, Guid referenceId, DSWEnvironmentType type)
        {
            switch (type)
            {
                case DSWEnvironmentType.Dossier:
                    return repository.Query(x => x.WorkflowInstance.Dossiers.Any(y => y.UniqueId == referenceId && (x.Status == WorkflowStatus.Progress || x.Status == WorkflowStatus.Todo)), optimization: true)
                        .SelectAsQueryable();
                case DSWEnvironmentType.Fascicle:
                    return repository.Query(x => x.WorkflowInstance.Fascicles.Any(y => y.UniqueId == referenceId && (x.Status == WorkflowStatus.Progress || x.Status == WorkflowStatus.Todo)), optimization: true)
                        .SelectAsQueryable();
                default:
                    ICollection<WorkflowActivity> emptyActivity = new List<WorkflowActivity>();
                    return emptyActivity.AsQueryable();
            }

        }

        public static IQueryable<WorkflowActivity> GetAuthorized(this IRepository<WorkflowActivity> repository,
            WorkflowActivityFinderModel finder, string account, bool optimization = true)
        {
            IQueryable<WorkflowActivity> workflowActivities = repository
                .Query(x => x.WorkflowAuthorizations.Any(y => y.Account.ToLower() == account.ToLower()), optimization: optimization)
                .Include(x => x.DocumentUnitReferenced.Category)
                .Include(x => x.DocumentUnitReferenced.Container)
                .OrderBy(o => o.OrderByDescending(oo => oo.RegistrationDate))
                .SelectAsQueryable();


            if (finder != null)
            {
                if (finder.IdWorkflowActivity.HasValue)
                {
                    workflowActivities = workflowActivities.Where(x => x.UniqueId == finder.IdWorkflowActivity.Value);
                }
                if (!string.IsNullOrWhiteSpace(finder.RegistrationUser))
                {
                    workflowActivities = workflowActivities.Where(x => x.RegistrationUser.Contains(finder.RegistrationUser.ToLower()));
                }
                if (finder.AuthorizeDateFrom.HasValue)
                {
                    workflowActivities = workflowActivities.Where(x => x.RegistrationDate >= finder.AuthorizeDateFrom.Value);
                }
                if (finder.AuthorizeDateTo.HasValue)
                {
                    workflowActivities = workflowActivities.Where(x => x.RegistrationDate <= finder.AuthorizeDateTo.Value);
                }
                if (!string.IsNullOrEmpty(finder.RequestorUsername))
                {
                    workflowActivities = workflowActivities.Where(x => x.WorkflowProperties.Any(y => y.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER && y.ValueString.Contains(finder.RequestorUsername)));
                }
                if (!string.IsNullOrEmpty(finder.RequestorRoleName))
                {
                    workflowActivities = workflowActivities.Where(x => x.DocumentUnitReferenced != null && x.DocumentUnitReferenced.Container.Name.Contains(finder.RequestorRoleName));
                }
                if (finder.Status.HasValue)
                {
                    WorkflowStatus workflowStatus = (WorkflowStatus)finder.Status.Value;
                    workflowActivities = workflowActivities.Where(x => x.WorkflowInstance.Status == workflowStatus);
                }

                if (!string.IsNullOrEmpty(finder.Note))
                {
                    workflowActivities = workflowActivities.Where(x => x.Note.Contains(finder.Note));
                }

                if (!string.IsNullOrEmpty(finder.Subject))
                {
                    workflowActivities = workflowActivities.Where(x => x.Subject.Contains(finder.Subject));
                }

                if (finder.Skip != null && finder.Top != null)
                {
                    workflowActivities = workflowActivities.Skip(finder.Skip.Value).Take(finder.Top.Value);
                }
            }

            return workflowActivities;
        }

        public static int CountAuthorizations(this IRepository<WorkflowActivity> repository, string account, Guid uniqueId, bool optimization = true)
        {
            int workflowActivitiesCount = repository
                .Query(optimization: optimization)
                .Include(x => x.WorkflowAuthorizations)
                .SelectAsQueryable()
                .FirstOrDefault(x => x.UniqueId == uniqueId)
                .WorkflowAuthorizations.Count(x => x.Account.ToLower() == account.ToLower());
            return workflowActivitiesCount;
        }

        public static WorkflowActivity GetWorkflowActivityByDoumentUnitId(this IRepository<WorkflowActivity> repository, Guid idDocumentUnit, string account, 
            bool optimization = true)
        {
            IQueryable<WorkflowActivity> workflowActivities = repository
                .Query(optimization: optimization)
                .Include(x => x.DocumentUnitReferenced.Category)
                .SelectAsQueryable()
                .Where(x => x.DocumentUnitReferenced.UniqueId == idDocumentUnit &&
                    (x.Status == WorkflowStatus.Progress || x.Status == WorkflowStatus.Todo) &&
                    x.WorkflowAuthorizations.Any(y => y.Account.ToLower() == account.ToLower()));
            if (workflowActivities.Count() == 1)
            {
                return workflowActivities.First();
            }

            return null;
        }

        public static WorkflowActivity GetLastWorkflowActivityByDoumentUnitId(this IRepository<WorkflowActivity> repository, Guid DocumentUnitId, string account, bool optimization = true)
        {
            WorkflowActivity workflowActivity = repository
                .Query(x => x.DocumentUnitReferenced.UniqueId == DocumentUnitId &&
                    (x.Status == WorkflowStatus.Done) &&
                    x.WorkflowAuthorizations.Any(y => y.Account.ToLower() == account.ToLower()), optimization: optimization)
                .Include(x => x.DocumentUnitReferenced.Category)
                .OrderBy(o => o.OrderByDescending(c => c.LastChangedDate))
                .Top(1).FirstOrDefault();

            return workflowActivity;
        }

        public static int CountUserAuthorized(this IRepository<WorkflowActivity> repository, string account, WorkflowActivityFinderModel finder, bool optimization = true)
        {
            var workflowActivities = repository
                .Query(optimization: optimization)
                .Include(x => x.WorkflowAuthorizations)
                .SelectAsQueryable()
                .Where(x => x.WorkflowAuthorizations.Any(y => y.Account.ToLower() == account.ToLower()));
            if (finder != null)
            {
                if (finder.IdWorkflowActivity.HasValue)
                {
                    workflowActivities = workflowActivities.Where(x => x.UniqueId == finder.IdWorkflowActivity.Value);
                }
                if (!string.IsNullOrWhiteSpace(finder.RegistrationUser))
                {
                    workflowActivities = workflowActivities.Where(x => x.RegistrationUser.Contains(finder.RegistrationUser.ToLower()));
                }
                if (finder.AuthorizeDateFrom.HasValue)
                {
                    workflowActivities = workflowActivities.Where(x => x.RegistrationDate >= finder.AuthorizeDateFrom.Value);
                }
                if (finder.AuthorizeDateTo.HasValue)
                {
                    workflowActivities = workflowActivities.Where(x => x.RegistrationDate <= finder.AuthorizeDateTo.Value);
                }
                if (finder.RequestorUsername != null && finder.RequestorUsername != string.Empty)
                {
                    workflowActivities = workflowActivities.Where(x => x.WorkflowProperties.Any(y => y.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER && y.ValueString.Contains(finder.RequestorUsername)));
                }
                if (finder.Status.HasValue)
                {
                    WorkflowStatus workflowStatus = (WorkflowStatus)finder.Status.Value;
                    workflowActivities = workflowActivities.Where(x => x.WorkflowInstance.Status == workflowStatus);
                }

                if (finder.Note != null && finder.Note != string.Empty)
                {
                    workflowActivities = workflowActivities.Where(x => x.Note.Contains(finder.Note));
                }

                if (finder.Subject != null && finder.Subject != string.Empty)
                {
                    workflowActivities = workflowActivities.Where(x => x.Subject.Contains(finder.Subject));
                }
            }
            return workflowActivities.Count();
        }
    }
}
