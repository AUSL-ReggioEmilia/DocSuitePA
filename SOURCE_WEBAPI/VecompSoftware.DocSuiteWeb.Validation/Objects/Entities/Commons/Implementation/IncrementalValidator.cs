using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class IncrementalValidator : ObjectValidator<Incremental, IncrementalValidator>, IIncrementalValidator
    {
        #region [ Constructor ]
        public IncrementalValidator(ILogger logger, IIncrementalValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region[ Properties ]
        public Guid UniqueId { get; set; }
        public string Name { get; set; }
        public int? IncrementalValue { get; set; }
        #endregion

        #region [ Navigation Properties ]          
        #endregion
    }
}