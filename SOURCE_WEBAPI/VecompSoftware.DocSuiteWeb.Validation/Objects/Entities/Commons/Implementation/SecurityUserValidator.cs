using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class SecurityUserValidator : ObjectValidator<SecurityUser, SecurityUserValidator>, ISecurityUserValidator
    {
        #region [ Constructor ]
        public SecurityUserValidator(ILogger logger, ISecurityUserValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }

        #endregion

        #region [ Properties ]
        public int EntityId { get; set; }
        /// <summary>
        /// Account name
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// Descrizione 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Dominio utente
        /// </summary>
        public string UserDomain { get; set; }
        public Guid UniqueId { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Collezione di ruoli per il tavolo
        /// </summary>
        public SecurityGroup Group { get; set; }

        /// <summary>
        /// Collezione di ruoli per il tavolo
        /// </summary>
        public ICollection<DeskRoleUser> DeskRoleUsers { get; set; }


        /// <summary>
        /// 
        /// </summary>
        //public ICollection<PECMailBoxUsers> PECMailBoxUsers { get; set; }
        #endregion
    }
}
