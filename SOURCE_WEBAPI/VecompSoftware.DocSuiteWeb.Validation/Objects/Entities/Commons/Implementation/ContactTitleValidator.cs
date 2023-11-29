using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class ContactTitleValidator : ObjectValidator<ContactTitle, ContactTitleValidator>, IContactTitleValidator
    {
        #region [ Constructor ]
        public ContactTitleValidator(ILogger logger, IContactTitleValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }

        #endregion

        #region[ Properties ]
        public int EntityId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool isActive { get; set; }
        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<ProtocolContactManual> ProtocolContactManuals { get; set; }

        #endregion

    }
}