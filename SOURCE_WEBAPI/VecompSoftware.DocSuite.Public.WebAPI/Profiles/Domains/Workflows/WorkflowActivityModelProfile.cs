using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers.Workflow;
using WorkflowStatus = VecompSoftware.DocSuiteWeb.Model.Entities.Workflows.WorkflowStatus;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Workflows
{
    public class WorkflowActivityModelProfile : Profile
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly Security.ISecurity _security;

        #endregion

        #region [ Constructor ]

        public WorkflowActivityModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            CreateMap<WorkflowActivity, WorkflowActivityModel>()
                .AfterMap((src, dest) =>
                {
                    IQueryable<WorkflowProperty> properties = _unitOfWork.Repository<WorkflowProperty>().Queryable(true).Where(f => f.WorkflowActivity.UniqueId == src.UniqueId &&
                    (f.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER || f.Name == WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS || f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION ||
                     f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_MOTIVATION || f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_REFERENCE_MODEL || f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_REFERENCE_MODEL ||
                     f.Name == WorkflowPropertyHelper.DSW_FIELD_RECIPIENT_POSITION));
                    src.WorkflowInstance = _unitOfWork.Repository<WorkflowInstance>()
                        .Query(x => x.WorkflowActivities.Any(xx => xx.UniqueId == src.UniqueId), true)
                        .Include(c => c.WorkflowRepository).Select().Single();
                    dest.ActivityName = src.Name;
                    dest.IdDocumentUnit = src.DocumentUnitReferenced?.UniqueId ?? null;
                    dest.IdWorkflowActivity = src.UniqueId;
                    dest.IdWorkflowInstance = src.WorkflowInstance.UniqueId;
                    dest.Status = (WorkflowStatus)src.WorkflowInstance.Status;
                    dest.WorkflowName = src.WorkflowInstance.WorkflowRepository.Name;
                    dest.RequestedDate = src.RegistrationDate;
                    dest.CategoryName = src.DocumentUnitReferenced?.Category?.Name;
                    dest.AuthorizationRole = src.DocumentUnitReferenced?.Container?.Name;
                    dest.RequestorUsername = GetRequestorUsername(src, properties);
                    dest.AuthorizationUser = GetAuthorizationUser(properties);
                    dest.RequestMotivation = GetStartMotivation(properties);
                    dest.WorkflowStartReferenceModel = GetStartReferenceModel(properties);
                    DateTimeOffset? endMotivationDate;
                    dest.EndMotivation = GetEndMotivation(properties, out endMotivationDate);
                    dest.EndMotivationDate = endMotivationDate;
                    dest.WorkflowEndReferenceModel = GetEndReferenceModel(properties);
                });
        }

        #endregion

        #region [ Helpers ]

        private WorkflowAccount GetRequestorUsername(WorkflowActivity src, IQueryable<WorkflowProperty> properties)
        {
            WorkflowProperty workflowProperty = properties.FirstOrDefault(x => x.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER);
            return workflowProperty == null
                ? new WorkflowAccount { AccountName = src.RegistrationUser }
                : JsonConvert.DeserializeObject<WorkflowAccount>(workflowProperty.ValueString);
        }

        private WorkflowAccount GetAuthorizationUser(IQueryable<WorkflowProperty> properties)
        {
            WorkflowProperty workflowProperty = properties.FirstOrDefault(x => x.Name == WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS);
            WorkflowProperty workflowPropertyPosition = properties.FirstOrDefault(x => x.Name == WorkflowPropertyHelper.DSW_FIELD_RECIPIENT_POSITION);
            int position = 0;
            if (workflowPropertyPosition != null && workflowPropertyPosition.ValueInt.HasValue)
            {
                position = (int)workflowPropertyPosition.ValueInt.Value;
            }
            WorkflowAccount workflowAccount = workflowProperty == null
                ? null
                : JsonConvert.DeserializeObject<List<WorkflowAccount>>(workflowProperty.ValueString).ElementAtOrDefault(position);
            return workflowAccount;
        }

        private string GetStartMotivation(IQueryable<WorkflowProperty> properties)
        {
            WorkflowProperty workflowProperty = properties.FirstOrDefault(x => x.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION);
            return workflowProperty == null ? string.Empty : workflowProperty.ValueString;
        }

        private string GetEndMotivation(IQueryable<WorkflowProperty> properties, out DateTimeOffset? endMotivationDate)
        {
            WorkflowProperty workflowProperty = properties.FirstOrDefault(x => x.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_MOTIVATION);
            endMotivationDate = workflowProperty == null ? default(DateTimeOffset?) : workflowProperty.RegistrationDate;
            return workflowProperty == null ? string.Empty : workflowProperty.ValueString;
        }

        private WorkflowReferenceModel GetStartReferenceModel(IQueryable<WorkflowProperty> properties)
        {
            WorkflowProperty workflowProperty = properties.FirstOrDefault(x => x.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_REFERENCE_MODEL);
            string documentsSerialized = workflowProperty == null
                ? string.Empty
                : workflowProperty.ValueString;
            WorkflowReferenceModel workflowReferenceModel = (documentsSerialized == string.Empty || documentsSerialized == null)
                ? null
                : JsonConvert.DeserializeObject<WorkflowReferenceModel>(documentsSerialized);
            return workflowReferenceModel;
        }

        private WorkflowReferenceModel GetEndReferenceModel(IQueryable<WorkflowProperty> properties)
        {
            WorkflowProperty workflowProperty = properties.FirstOrDefault(x => x.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_REFERENCE_MODEL);
            string documentsSerialized = workflowProperty == null
                ? string.Empty
                : workflowProperty.ValueString;
            WorkflowReferenceModel workflowReferenceModel = (documentsSerialized == string.Empty || documentsSerialized == null)
                ? null
                : JsonConvert.DeserializeObject<WorkflowReferenceModel>(documentsSerialized);
            return workflowReferenceModel;
        }
        #endregion
    }
}