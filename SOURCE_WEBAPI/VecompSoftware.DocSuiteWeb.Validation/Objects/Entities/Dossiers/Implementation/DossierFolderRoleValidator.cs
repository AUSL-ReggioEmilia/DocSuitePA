using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers
{
    public class DossierFolderRoleValidator : ObjectValidator<DossierFolderRole, DossierFolderRoleValidator>, IDossierFolderRoleValidator
    {
        #region [ Constructor ]

        public DossierFolderRoleValidator(ILogger logger, IDossierFolderRoleValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }
        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public AuthorizationRoleType RoleAuthorizationType { get; set; }
        public DossierRoleStatus Status { get; set; }
        public bool IsMaster { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Role Role { get; set; }
        public DossierFolder DossierFolder { get; set; }

        #endregion
    }
}
