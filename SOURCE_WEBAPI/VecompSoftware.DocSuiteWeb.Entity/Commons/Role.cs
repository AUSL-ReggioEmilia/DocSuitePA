using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{

    public class Role : DSWBaseEntity
    {
        #region [ Constructor ]
        public Role() : this(Guid.NewGuid()) { }

        public Role(Guid uniqueId)
            : base(uniqueId)
        {
            WorkflowRepositories = new HashSet<WorkflowRepository>();
            CollaborationUsers = new HashSet<CollaborationUser>();
            Contacts = new HashSet<Contact>();
            RootContacts = new HashSet<Contact>();
            OChartItems = new HashSet<OChartItem>();
            ProtocolRoles = new HashSet<ProtocolRole>();
            RoleGroups = new HashSet<RoleGroup>();
            RoleUsers = new HashSet<RoleUser>();
            ResolutionRoles = new HashSet<ResolutionRole>();
            ProtocolRoleUsers = new HashSet<ProtocolRoleUser>();
            WorkflowInstanceRoles = new HashSet<WorkflowInstanceRole>();
            FascicleRoles = new HashSet<FascicleRole>();
            TemplateCollaborationUsers = new HashSet<TemplateCollaborationUser>();
            TemplateCollaborations = new HashSet<TemplateCollaboration>();
            DossierRoles = new HashSet<DossierRole>();
            DossierFolderRoles = new HashSet<DossierFolderRole>();
            UDSAuthorizations = new HashSet<UDSRole>();
            //Mailboxes = new HashSet<PECMailBox>();
            TransparentAdministrationMonitorLogs = new HashSet<TransparentAdministrationMonitorLog>();
            Tenants = new HashSet<Tenant>();
            Processes = new HashSet<Process>();
        }
        #endregion

        #region [ Properties ]

        public string Name { get; set; }

        public byte IsActive { get; set; }

        public DateTime? ActiveFrom { get; set; }

        public DateTime? ActiveTo { get; set; }

        public string FullIncrementalPath { get; set; }

        public byte Collapsed { get; set; }

        public string EMailAddress { get; set; }

        public string ServiceCode { get; set; }

        public short IdRoleTenant { get; set; }

        public Guid TenantId { get; set; }


        #endregion

        #region [ Navigation Properties ]
        public Role Father { get; set; }

        public virtual ICollection<TemplateCollaborationUser> TemplateCollaborationUsers { get; set; }

        public virtual ICollection<WorkflowRepository> WorkflowRepositories { get; set; }

        public virtual ICollection<CollaborationUser> CollaborationUsers { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }

        public virtual ICollection<Contact> RootContacts { get; set; }

        public virtual ICollection<OChartItem> OChartItems { get; set; }

        public virtual ICollection<RoleGroup> RoleGroups { get; set; }

        public virtual ICollection<ProtocolRole> ProtocolRoles { get; set; }

        public virtual ICollection<ProtocolRoleUser> ProtocolRoleUsers { get; set; }

        public virtual ICollection<ResolutionRole> ResolutionRoles { get; set; }

        public virtual ICollection<RoleUser> RoleUsers { get; set; }
        //public virtual ICollection<PECMailBox> Mailboxes { get; set; }

        public virtual ICollection<DocumentSeriesItemRole> DocumentSeriesItemRoles { get; set; }

        public virtual ICollection<WorkflowRoleMapping> WorkflowRoleMappings { get; set; }

        public virtual ICollection<WorkflowInstanceRole> WorkflowInstanceRoles { get; set; }

        public virtual ICollection<FascicleRole> FascicleRoles { get; set; }

        public virtual ICollection<TemplateCollaboration> TemplateCollaborations { get; set; }

        public virtual ICollection<DossierRole> DossierRoles { get; set; }

        public virtual ICollection<DossierFolderRole> DossierFolderRoles { get; set; }

        public virtual ICollection<UDSRole> UDSAuthorizations { get; set; }

        public virtual ICollection<TransparentAdministrationMonitorLog> TransparentAdministrationMonitorLogs { get; set; }
        public virtual ICollection<Tenant> Tenants { get; set; }

        public virtual ICollection<CategoryFascicleRight> CategoryFascicleRights { get; set; }

        public virtual ICollection<Process> Processes { get; set; }
        #endregion
    }
}
