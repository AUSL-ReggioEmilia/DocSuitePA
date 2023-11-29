using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class MetadataValueValidator : ObjectValidator<MetadataValue, MetadataValueValidator>, IMetadataValueValidator
    {
        #region [ Constructor ]
        public MetadataValueValidator(ILogger logger, IMetadataValueValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public MetadataPropertyType PropertyType { get; set; }
        public string Name { get; set; }
        public long? ValueInt { get; set; }
        public DateTime? ValueDate { get; set; }
        public double? ValueDouble { get; set; }
        public bool? ValueBoolean { get; set; }
        public Guid? ValueGuid { get; set; }
        public string ValueString { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public Fascicle Fascicle { get; set; }
        public Dossier Dossier { get; set; }
        #endregion

    }
}
