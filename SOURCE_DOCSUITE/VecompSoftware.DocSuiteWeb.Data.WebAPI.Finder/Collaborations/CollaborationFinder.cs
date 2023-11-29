using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;
using VecompSoftware.WebAPIManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VecompSoftware.Services.Logging;
using VecompSoftware.WebAPIManager.Exceptions;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.DTO.OData;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using System.Linq;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Collaborations
{
    public class CollaborationFinder : BaseWebAPIFinder<Collaboration, CollaborationModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        public string UserName { get; set; }

        public string Domain { get; set; }

        public CollaborationFinderActionType? CollaborationFinderActionType { get; set; }

        public CollaborationFinderFilterType CollaborationFinderFilterType { get; set; }

        public CollaborationFinderModel CollaborationFinderModel { get; set; }

        public bool FromPostMethod { get; set; }
        private bool FromFinderModel => CollaborationFinderModel != null;
        #endregion

        #region [ Constructor ]

        public CollaborationFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public CollaborationFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }

        #endregion

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            EnablePaging = false;

            if (CollaborationFinderModel.EntityId.HasValue)
            {
                odataQuery = odataQuery.Filter($"EntityId eq {CollaborationFinderModel.EntityId}");
            }

            return odataQuery;
        }

        public List<WebAPIDto<CollaborationModel>> GetFromPostMethod()
        {
            string odataFunction = string.Empty;
            if (CollaborationFinderActionType.HasValue)
            {
                switch (CollaborationFinderActionType)
                {
                    //Alla Visione/Firma
                    case Collaborations.CollaborationFinderActionType.AtVisionSign:
                        odataFunction = CommonDefinition.OData.CollaborationService.FX_GetAtVisionSignCollaborations;
                        break;

                    //Da Visionare/Firmare
                    case Collaborations.CollaborationFinderActionType.ToVisionSign:
                        switch (CollaborationFinderFilterType)
                        {
                            case CollaborationFinderFilterType.AllCollaborations:
                                CollaborationFinderModel.IsRequired = null;
                                break;

                            case CollaborationFinderFilterType.SignRequired:
                                CollaborationFinderModel.IsRequired = true;
                                break;

                            case CollaborationFinderFilterType.OnlyVision:
                                CollaborationFinderModel.IsRequired = false;
                                break;
                        }
                        odataFunction = CommonDefinition.OData.CollaborationService.FX_GetToVisionSignCollaborations;
                        break;
                    //Deleghe
                    case Collaborations.CollaborationFinderActionType.ToDelegateVisionSign:
                        odataFunction = CommonDefinition.OData.CollaborationService.FX_GetToVisionDelegateSignCollaborations;
                        break;

                    //Al Protocollo/Segreteria
                    case Collaborations.CollaborationFinderActionType.AtProtocolAdmission:
                        odataFunction = CommonDefinition.OData.CollaborationService.FX_GetAtProtocolAdmissionCollaborations;
                        break;

                    //Attività in corso
                    case Collaborations.CollaborationFinderActionType.Running:
                        switch (CollaborationFinderFilterType)
                        {
                            case CollaborationFinderFilterType.AllCollaborations:
                                odataFunction = CommonDefinition.OData.CollaborationService.FX_GetCurrentActivitiesAllCollaborations;
                                break;

                            case CollaborationFinderFilterType.ActiveCollaborations:
                                odataFunction = CommonDefinition.OData.CollaborationService.FX_GetCurrentActivitiesActiveCollaborations;
                                break;

                            case CollaborationFinderFilterType.PastCollaborations:
                                odataFunction = CommonDefinition.OData.CollaborationService.FX_GetCurrentActivitiesPastCollaborations;
                                break;
                        }
                        break;

                    //Da Protocollare/Gestire
                    case Collaborations.CollaborationFinderActionType.ToManage:
                        odataFunction = CommonDefinition.OData.CollaborationService.FX_GetToManageCollaborations;
                        break;

                    //Protocollati/Gestiti
                    case Collaborations.CollaborationFinderActionType.Registered:
                        odataFunction = CommonDefinition.OData.CollaborationService.FX_GetRegisteredCollaborations;
                        break;

                    //Miei check out
                    case Collaborations.CollaborationFinderActionType.CheckedOut:
                        odataFunction = CommonDefinition.OData.CollaborationService.FX_GetMyCheckedOutCollaborations;
                        break;
                }
            }

            WebApiHttpClient httpClient = GetWebAPIClient(CurrentTenant);
            CollaborationFinderModel.Skip = PageIndex;
            CollaborationFinderModel.Top = PageSize;
            string bodyQuery = JsonConvert.SerializeObject(CollaborationFinderModel, new StringEnumConverter());
            IEnumerable<CollaborationModel> results = httpClient.PostStringAsync<Collaboration>($"/{odataFunction}", bodyQuery).ResponseToModel<ODataModel<ICollection<CollaborationModel>>>().Value;
            
            return results.Select(collaborationModel => new WebAPIDto<CollaborationModel>
            {
                Entity = collaborationModel,
                TenantModel = CurrentTenant
            }).ToList();
        }

        public override int Count()
        {
            if (!FromFinderModel)
            {
                return base.Count();
            }

            return CurrentTenantExecutionWebAPI<Collaboration, int>((tenant) =>
            {
                string odataCountFunction = string.Empty;
                if (CollaborationFinderActionType.HasValue)
                {
                    switch (CollaborationFinderActionType)
                    {
                        //Alla Visione/Firma
                        case Collaborations.CollaborationFinderActionType.AtVisionSign:
                            odataCountFunction = CommonDefinition.OData.CollaborationService.FX_CountAtVisionSignCollaborations;
                            break;

                        //Da Visionare/Firmare
                        case Collaborations.CollaborationFinderActionType.ToVisionSign:
                            switch (CollaborationFinderFilterType)
                            {
                                case CollaborationFinderFilterType.AllCollaborations:
                                    CollaborationFinderModel.IsRequired = null;
                                    break;

                                case CollaborationFinderFilterType.SignRequired:
                                    CollaborationFinderModel.IsRequired = true;
                                    break;

                                case CollaborationFinderFilterType.OnlyVision:
                                    CollaborationFinderModel.IsRequired = false;
                                    break;
                            }
                            odataCountFunction = CommonDefinition.OData.CollaborationService.FX_CountToVisionSignCollaborations;
                            break;
                        //Deleghe
                        case Collaborations.CollaborationFinderActionType.ToDelegateVisionSign:
                            odataCountFunction = CommonDefinition.OData.CollaborationService.FX_CountToVisionDelegateSignCollaborations;
                            break;

                        //Al Protocollo/Segreteria
                        case Collaborations.CollaborationFinderActionType.AtProtocolAdmission:
                            odataCountFunction = CommonDefinition.OData.CollaborationService.FX_CountAtProtocolAdmissionCollaborations;
                            break;

                        //Attività in corso
                        case Collaborations.CollaborationFinderActionType.Running:
                            switch (CollaborationFinderFilterType)
                            {
                                case CollaborationFinderFilterType.AllCollaborations:
                                    odataCountFunction = CommonDefinition.OData.CollaborationService.FX_CountCurrentActivitiesAllCollaborations;
                                    break;

                                case CollaborationFinderFilterType.ActiveCollaborations:
                                    odataCountFunction = CommonDefinition.OData.CollaborationService.FX_CountCurrentActivitiesActiveCollaborations;

                                    break;

                                case CollaborationFinderFilterType.PastCollaborations:
                                    odataCountFunction = CommonDefinition.OData.CollaborationService.FX_CountCurrentActivitiesPastCollaborations;
                                    break;
                            }
                            break;

                        //Da Protocollare/Gestire
                        case Collaborations.CollaborationFinderActionType.ToManage:
                            odataCountFunction = CommonDefinition.OData.CollaborationService.FX_CountToManageCollaborations;
                            break;

                        //Protocollati/Gestiti
                        case Collaborations.CollaborationFinderActionType.Registered:
                            odataCountFunction = CommonDefinition.OData.CollaborationService.FX_CountRegisteredCollaborations;
                            break;

                        //Miei check out
                        case Collaborations.CollaborationFinderActionType.CheckedOut:
                            odataCountFunction = CommonDefinition.OData.CollaborationService.FX_CountMyCheckedOutCollaborations;
                            break;
                    }
                }

                WebApiHttpClient httpClient = GetWebAPIClient(CurrentTenant);
                string bodyQuery = JsonConvert.SerializeObject(CollaborationFinderModel, new StringEnumConverter());
                int result = httpClient.PostStringAsync<Collaboration>($"/{odataCountFunction}", bodyQuery).ResponseToModel<ODataModel<int>>().Value;
                return result;
            }, nameof(Count));
        }

        private TResult CurrentTenantExecutionWebAPI<TModel, TResult>(Func<TenantModel, TResult> func, string methodName)
        {
            string errorMessage = string.Concat("Errore nell'esecuzione del metodo ", methodName, " .");

            try
            {
                return func(CurrentTenant);
            }
            catch (Exception ex)
            {
                FileLogger.Error(Logger, errorMessage, ex);
                throw new WebAPIException<TResult>(ex.Message, ex);
            }
        }

        public void PopulateFinderModel(string propertyName, object propertyValue)
        {
            switch (propertyName)
            {
                case nameof(CollaborationFinderModel.MemorandumDate):
                    {
                        CollaborationFinderModel.MemorandumDate = (DateTimeOffset)propertyValue;
                        break;
                    }
                case nameof(CollaborationFinderModel.Object):
                    {
                        CollaborationFinderModel.Object = propertyValue.ToString();
                        break;
                    }
                case nameof(CollaborationFinderModel.Note):
                    {
                        CollaborationFinderModel.Note = propertyValue.ToString();
                        break;
                    }
                case nameof(CollaborationFinderModel.RegistrationName):
                    {
                        CollaborationFinderModel.RegistrationName = propertyValue.ToString();
                        break;
                    }
            }
        }

        public override void ResetDecoration()
        {
            UserName = string.Empty;
            Domain = string.Empty;
            CollaborationFinderActionType = Collaborations.CollaborationFinderActionType.AtProtocolAdmission;
            CollaborationFinderModel = new CollaborationFinderModel();
        }

        #endregion
    }
}
