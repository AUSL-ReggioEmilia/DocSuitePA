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
    public class MetadataRepositoryValidator : ObjectValidator<MetadataRepository, MetadataRepositoryValidator>, IMetadataRepositoryValidator
    {
        #region [ Constructor ]
        public MetadataRepositoryValidator(ILogger logger, IMetadataRepositoryValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
        }
        #endregion

        #region[ Properties ]
        public string Name { get; set; }
        public MetadataRepositoryStatus Status { get; set; }
        public string JsonMetadata { get; set; }
        public int Version { get; set; }
        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public DateTimeOffset DateFrom { get; set; }
        public DateTimeOffset? DateTo { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public ICollection<Fascicle> Fascicles { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Dossier> Dossiers { get; set; }
        #endregion

    }
}
