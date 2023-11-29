using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations
{
    public class CollaborationAggregateValidator : ObjectValidator<CollaborationAggregate, CollaborationAggregateValidator>, ICollaborationAggregateValidator
    {
        #region [ Constructor ]
        public CollaborationAggregateValidator(ILogger logger, ICollaborationAggregateValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string CollaborationDocumentType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Collaboration CollaborationFather { get; set; }
        public Collaboration CollaborationChild { get; set; }
        #endregion

    }
}
