using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS
{
    public class UDSRepositoryValidator : ObjectValidator<UDSRepository, UDSRepositoryValidator>, IUDSRepositoryValidator
    {
        #region [ Constructor ]
        public UDSRepositoryValidator(ILogger logger, IUDSRepositoryValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        {

            DocumentUnits = new HashSet<DocumentUnit>();
            PecMails = new HashSet<PECMail>();
            UDSTypologies = new HashSet<UDSTypology>();
            UDSLogs = new HashSet<UDSLog>();
            UDSRoles = new HashSet<UDSRole>();
            UDSUsers = new HashSet<UDSUser>();
        }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }

        public string Name { get; set; }

        public short SequenceCurrentYear { get; set; }

        public int SequenceCurrentNumber { get; set; }

        public string ModuleXML { get; set; }

        public short Version { get; set; }

        public DateTimeOffset ActiveDate { get; set; }

        public DateTimeOffset? ExpiredDate { get; set; }

        public UDSRepositoryStatus Status { get; set; }

        public string Alias { get; set; }

        public int DSWEnvironment { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Container Container { get; set; }
        public UDSSchemaRepository SchemaRepository { get; set; }
        public ICollection<DocumentUnit> DocumentUnits { get; set; }
        public ICollection<PECMail> PecMails { get; set; }
        public ICollection<UDSTypology> UDSTypologies { get; set; }
        public ICollection<UDSLog> UDSLogs { get; set; }
        public ICollection<UDSRole> UDSRoles { get; set; }
        public ICollection<UDSUser> UDSUsers { get; set; }
        #endregion
    }
}
