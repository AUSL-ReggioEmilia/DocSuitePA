using System;
using System.Collections.Generic;
using System.Text;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Templates
{
    public class TemplateCollaborationFinder : BaseWebAPIFinder<TemplateCollaboration, TemplateCollaboration>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public TemplateCollaborationStatus? Status { get; set; }
        public bool? OnlyAuthorized { get; set; }
        public bool? Locked { get; set; }
        public string UserName { get; set; }
        public string Domain { get; set; }
        public string DocumentType { get; set; }
        public bool? ExpandProperties { get; set; }
        public int? IdRole { get; set; }
        public string Name { get; set; }
        #endregion

        #region [ Constructor ]
        public TemplateCollaborationFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public TemplateCollaborationFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {

        }
        #endregion

        #region [ Methods ]
        public override void ResetDecoration()
        {
            UniqueId = null;
            Status = null;
            OnlyAuthorized = null;
            Locked = null;
            UserName = string.Empty;
            Domain = string.Empty;
            DocumentType = string.Empty;
            Name = string.Empty;
            ExpandProperties = null;
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (ExpandProperties.HasValue && ExpandProperties.Value)
            {
                odataQuery = odataQuery.Expand("TemplateCollaborationUsers($expand=Role)")
                    .Expand("TemplateCollaborationDocumentRepositories")
                    .Expand("Roles");
            }

            if (Status.HasValue)
            {
                if ((OnlyAuthorized.HasValue && OnlyAuthorized.Value) || (!string.IsNullOrEmpty(UserName) && IdRole != null))
                {
                    odataQuery = odataQuery.Filter(string.Concat("Status eq VecompSoftware.DocSuiteWeb.Model.Entities.Templates.TemplateCollaborationStatus'", (int)Status.Value, "'"));
                }
                else
                {
                    odataQuery = odataQuery.Filter(string.Concat("Status eq VecompSoftware.DocSuiteWeb.Entity.Templates.TemplateCollaborationStatus'", (int)Status.Value, "'"));
                }
            }

            if (OnlyAuthorized.HasValue && OnlyAuthorized.Value)
            {
                odataQuery = odataQuery.Function(string.Format(CommonDefinition.OData.TemplateCollaborationService.FX_GetAuthorizedTemplates, UserName, Domain));
            }

            if (!string.IsNullOrEmpty(UserName) && IdRole != null)
            {
                odataQuery = odataQuery.Function(string.Format(CommonDefinition.OData.TemplateCollaborationService.FX_GetInvalidatingTemplatesByRoleUserAccount, UserName, Domain, IdRole, DocumentType));
            }

            if (Locked.HasValue)
            {
                odataQuery.Filter(string.Concat("IsLocked eq ", Locked.ToString().ToLower()));
            }

            if (!string.IsNullOrEmpty(DocumentType))
            {
                StringBuilder filterBuilder = new StringBuilder("(");
                filterBuilder.Append($"DocumentType eq '{DocumentType} '");
                if (DocumentType.Equals("UDS", StringComparison.InvariantCultureIgnoreCase))
                {
                    filterBuilder.Append($" or (startswith(DocumentType, '1') and length(DocumentType) gt 2)");
                }
                filterBuilder.Append(")");
                
                odataQuery.Filter(filterBuilder.ToString());
            }

            if (!string.IsNullOrEmpty(Name))
            {
                odataQuery.Filter(string.Concat("Name eq '", Name.Replace("'", "''"), "'"));
            }

            return base.DecorateFinder(odataQuery);
        }
        #endregion        
    }
}
