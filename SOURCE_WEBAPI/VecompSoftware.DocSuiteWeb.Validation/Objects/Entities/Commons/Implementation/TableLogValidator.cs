using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class TableLogValidator : ObjectValidator<TableLog, TableLogValidator>, ITableLogValidator
    {
        #region [ Constructor ]
        public TableLogValidator(ILogger logger, ITableLogValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }

        public int? LoggedEntityId { get; set; }

        public Guid? LoggedEntityUniqueId { get; set; }

        public string TableName { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string SystemComputer { get; set; }

        public string RegistrationUser { get; set; }

        public TableLogEvent LogType { get; set; }

        public string LogDescription { get; set; }

        public string Hash { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }

        #endregion
    }
}
