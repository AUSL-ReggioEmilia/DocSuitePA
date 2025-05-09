﻿using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS
{
    public class UDSCollaborationValidator : ObjectValidator<UDSCollaboration, UDSCollaborationValidator>, IUDSCollaborationValidator
    {
        #region [ Constructor ]
        public UDSCollaborationValidator(ILogger logger, IUDSCollaborationValidatorMapper mapper,
            IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public Guid IdUDS { get; set; }
        public int Environment { get; set; }
        public UDSRelationType RelationType { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public UDSRepository Repository { get; set; }
        public Collaboration Collaboration { get; set; }
        #endregion
    }
}
