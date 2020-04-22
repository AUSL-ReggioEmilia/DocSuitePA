using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class ContactPlaceNameValidator : ObjectValidator<ContactPlaceName, ContactPlaceNameValidator>, IContactPlaceNameValidator
    {
        #region [ Constructor ]
        public ContactPlaceNameValidator(ILogger logger, IContactPlaceNameValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region[ Properties ]
        public short EntityShortId { get; set; }
        public Guid UniqueId { get; set; }
        public string Description { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<ProtocolContactManual> ProtocolContactManuals { get; set; }
        #endregion

    }
}