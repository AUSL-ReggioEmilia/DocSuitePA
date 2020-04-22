using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS
{
    public class UDSSchemaRepositoryValidator : ObjectValidator<UDSSchemaRepository, UDSSchemaRepositoryValidator>, IUDSSchemaRepositoryValidator
    {
        #region [ Constructor ]
        public UDSSchemaRepositoryValidator(ILogger logger, IUDSSchemaRepositoryValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string SchemaXML { get; set; }

        public short Version { get; set; }

        public DateTimeOffset ActiveDate { get; set; }

        public DateTimeOffset? ExpiredDate { get; set; }

        #endregion
    }
}
