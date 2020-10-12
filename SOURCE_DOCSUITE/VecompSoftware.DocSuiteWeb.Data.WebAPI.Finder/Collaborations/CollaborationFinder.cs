using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;
using VecompSoftware.WebAPIManager;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Collaborations
{
    public class CollaborationFinder : BaseWebAPIFinder<Collaboration, CollaborationModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        public string UserName { get; set; }

        public string Domain { get; set; }

        public string DocumentType { get; set; }

        public DateTimeOffset? DateFrom { get; set; }

        public DateTimeOffset? DateTo { get; set; }

        public CollaborationFinderActionType? CollaborationFinderActionType { get; set; }

        public CollaborationFinderFilterType CollaborationFinderFilterType { get; set; }

        public int? Incremental { get; set; }

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
            if (Incremental.HasValue)
            {
                odataQuery = odataQuery.Filter($"EntityId eq {Incremental.Value}");
            }

            //Tipologia documento
            if (!string.IsNullOrEmpty(DocumentType))
            {
                string filter = $"DocumentType eq '{DocumentType}'";
                if (DocumentType.Equals("UDS", StringComparison.InvariantCultureIgnoreCase))
                {
                    filter = string.Concat(filter, " or (startswith(DocumentType, '1') and length(DocumentType) gt 2)");
                }
                odataQuery = odataQuery.Filter(filter);
            }

            if (CollaborationFinderActionType.HasValue)
            {
                switch (CollaborationFinderActionType)
                {
                    //Alla Visione/Firma
                    case Collaborations.CollaborationFinderActionType.AtVisionSign:
                        odataQuery = odataQuery.Function(CommonDefinition.OData.CollaborationService.FX_GetAtVisionSignCollaborations);
                        break;

                    //Da Visionare/Firmare
                    case Collaborations.CollaborationFinderActionType.ToVisionSign:
                        switch (CollaborationFinderFilterType)
                        {
                            case CollaborationFinderFilterType.AllCollaborations:
                                odataQuery = odataQuery.Function(CommonDefinition.OData.CollaborationService.FX_GetToVisionSignCollaborations);
                                break;

                            case CollaborationFinderFilterType.SignRequired:
                                odataQuery = odataQuery.Function(CommonDefinition.OData.CollaborationService.FX_GetToVisionSignRequiredCollaborations);
                                break;

                            case CollaborationFinderFilterType.OnlyVision:
                                odataQuery = odataQuery.Function(CommonDefinition.OData.CollaborationService.FX_GetToVisionSignNoRequiredCollaborations);
                                break;
                        }
                        break;
                    //Deleghe
                    case Collaborations.CollaborationFinderActionType.ToDelegateVisionSign:
                        odataQuery = odataQuery.Function(CommonDefinition.OData.CollaborationService.FX_GetToVisionDelegateSignCollaborations);
                        break;

                    //Al Protocollo/Segreteria
                    case Collaborations.CollaborationFinderActionType.AtProtocolAdmission:
                        odataQuery = odataQuery.Function(CommonDefinition.OData.CollaborationService.FX_GetAtProtocolAdmissionCollaborations);
                        break;

                    //Attività in corso
                    case Collaborations.CollaborationFinderActionType.Running:
                        switch (CollaborationFinderFilterType)
                        {
                            case CollaborationFinderFilterType.AllCollaborations:
                                odataQuery = odataQuery.Function(CommonDefinition.OData.CollaborationService.FX_GetCurrentActivitiesAllCollaborations);
                                break;

                            case CollaborationFinderFilterType.ActiveCollaborations:
                                odataQuery = odataQuery.Function(CommonDefinition.OData.CollaborationService.FX_GetCurrentActivitiesActiveCollaborations);

                                break;

                            case CollaborationFinderFilterType.PastCollaborations:
                                odataQuery = odataQuery.Function(CommonDefinition.OData.CollaborationService.FX_GetCurrentActivitiesPastCollaborations);
                                break;
                        }
                        break;

                    //Da Protocollare/Gestire
                    case Collaborations.CollaborationFinderActionType.ToManage:
                        odataQuery = odataQuery.Function(CommonDefinition.OData.CollaborationService.FX_GetToManageCollaborations);
                        break;

                    //Protocollati/Gestiti
                    case Collaborations.CollaborationFinderActionType.Registered:
                        odataQuery = odataQuery.Function(string.Format(CommonDefinition.OData.CollaborationService.FX_GetRegisteredCollaborations, DateFrom.Value.ToString(ODataDateConversion), DateTo.Value.ToString(ODataDateConversion)));
                        break;

                    //Miei check out
                    case Collaborations.CollaborationFinderActionType.CheckedOut:
                        odataQuery = odataQuery.Function(CommonDefinition.OData.CollaborationService.FX_GetMyCheckedOutCollaborations);
                        break;
                }
            }

            return odataQuery;
        }

        public override void ResetDecoration()
        {
            UserName = string.Empty;
            Domain = string.Empty;
            DocumentType = string.Empty;
            DateFrom = null;
            DateTo = null;
            CollaborationFinderActionType = Collaborations.CollaborationFinderActionType.AtProtocolAdmission;
        }

        #endregion
    }
}
