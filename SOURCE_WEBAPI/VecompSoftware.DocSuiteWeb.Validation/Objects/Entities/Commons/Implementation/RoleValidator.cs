using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class RoleValidator : ObjectValidator<Role, RoleValidator>, IRoleValidator
    {
        #region [ Constructor ]
        public RoleValidator(ILogger logger, IRoleValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }

        #endregion

        #region [ Properties ]
        public short EntityShortId { get; set; }
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string FullIncrementalPath { get; set; }

        public bool Collapsed { get; set; }

        public string EMailAddress { get; set; }
        public string ServiceCode { get; set; }
        public Guid UniqueId { get; set; }
        public RoleTypology RoleTypology { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Role Father { get; set; }
        public TenantAOO TenantAOO { get; set; }

        public ICollection<WorkflowRepository> WorkflowRepositories { get; set; }

        public ICollection<CollaborationUser> CollaborationUsers { get; set; }

        public ICollection<Contact> Contacts { get; set; }

        public ICollection<Contact> RootContacts { get; set; }

        public ICollection<OChartItem> OChartItems { get; set; }

        public ICollection<ProtocolRole> ProtocolRoles { get; set; }

        public ICollection<ProtocolRoleUser> ProtocolRoleUsers { get; set; }

        public ICollection<RoleGroup> RoleGroups { get; set; }

        public ICollection<RoleUser> RoleUsers { get; set; }

        public ICollection<ResolutionRole> ResolutionRoles { get; set; }

        public ICollection<WorkflowInstanceRole> WorkflowInstanceRoles { get; set; }

        public ICollection<FascicleRole> FascicleRoles { get; set; }

        public ICollection<TemplateCollaboration> TemplateCollaborations { get; set; }

        public ICollection<TemplateCollaborationUser> TemplateCollaborationUsers { get; set; }

        public ICollection<DossierRole> DossierRoles { get; set; }

        //public ICollection<PECMailBox> Mailboxes { get; set; }

        public ICollection<DossierFolderRole> DossierFolderRoles { get; set; }

        public ICollection<UDSRole> UDSAuthorizations { get; set; }
        #endregion
    }
}
