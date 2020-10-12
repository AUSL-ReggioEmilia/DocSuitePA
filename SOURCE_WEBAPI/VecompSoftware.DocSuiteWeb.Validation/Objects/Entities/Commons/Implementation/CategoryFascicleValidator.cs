using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class CategoryFascicleValidator : ObjectValidator<CategoryFascicle, CategoryFascicleValidator>, ICategoryFascicleValidator
    {
        #region [ Constructor ]
        public CategoryFascicleValidator(ILogger logger, ICategoryFascicleValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region[ Properties ]

        public Guid UniqueId { get; set; }
        public int DSWEnvironment { get; set; }
        public FascicleType FascicleType { get; set; }
        public string CustomActions { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }


        #endregion

        #region [ Navigation Properties ]
        public Category Category { get; set; }
        public FasciclePeriod FasciclePeriod { get; set; }
        public Contact Manager { get; set; }
        #endregion
    }
}