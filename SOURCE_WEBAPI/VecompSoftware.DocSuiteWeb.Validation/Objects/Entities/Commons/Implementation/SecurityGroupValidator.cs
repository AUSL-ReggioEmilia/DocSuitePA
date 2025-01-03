﻿using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class SecurityGroupValidator : ObjectValidator<SecurityGroup, SecurityGroupValidator>, ISecurityGroupValidator
    {
        #region [ Constructor ]
        public SecurityGroupValidator(ILogger logger, ISecurityGroupValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }

        #endregion

        #region [ Properties ]
        public int EntityId { get; set; }
        /// <summary>
        /// Nome del gruppo
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// Descrizione del log
        /// </summary>
        public string LogDescription { get; set; }
        public bool IsAllUsers { get; set; }

        public Guid UniqueId { get; set; }
        #endregion

        #region [ Navigation Properties ]

        /// <summary>
        /// Gruppo padre
        /// </summary>
        public SecurityGroup GroupFather { get; set; }

        /// <summary>
        /// Gruppo padre
        /// </summary>
        public ICollection<SecurityGroup> GroupChildren { get; set; }

        /// <summary>
        /// Collezione degli utenti
        /// </summary>
        public ICollection<SecurityUser> SecurityUsers { get; set; }

        public ICollection<ContainerGroup> ContainerGroups { get; set; }

        public ICollection<RoleGroup> RoleGroups { get; set; }

        #endregion
    }
}
