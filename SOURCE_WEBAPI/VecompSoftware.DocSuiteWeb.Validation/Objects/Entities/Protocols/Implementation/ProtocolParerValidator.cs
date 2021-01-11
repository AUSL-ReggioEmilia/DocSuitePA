using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols
{
    public class ProtocolParerValidator : ObjectValidator<ProtocolParer, ProtocolParerValidator>, IProtocolParerValidator
    {
        #region [ Constructor ]
        public ProtocolParerValidator(ILogger logger, IProtocolParerValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public short Year { get; set; }
        public int Number { get; set; }
        public DateTime? ArchiviedDate { get; set; }
        public string ParerUri { get; set; }
        public bool? HasError { get; set; }
        public short? IsForArchive { get; set; }
        public string LastError { get; set; }
        public DateTime? LastSendDate { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string LastChangedUser { get; set; }
        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Protocol Protocol { get; set; }
        #endregion


    }
}
