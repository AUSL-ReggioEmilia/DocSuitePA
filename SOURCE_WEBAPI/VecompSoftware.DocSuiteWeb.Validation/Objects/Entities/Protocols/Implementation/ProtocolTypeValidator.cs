using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols
{
    public class ProtocolTypeValidator : ObjectValidator<ProtocolType, ProtocolTypeValidator>, IProtocolTypeValidator
    {
        #region [ Constructor ]
        public ProtocolTypeValidator(ILogger logger, IProtocolTypeValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region [ Properties ]
        public short EntityShortId { get; set; }
        public string Description { get; set; }
        public Guid UniqueId { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public ICollection<Container> Containers { get; set; }
        public ICollection<Protocol> Protocols { get; set; }
        #endregion
    }
}
