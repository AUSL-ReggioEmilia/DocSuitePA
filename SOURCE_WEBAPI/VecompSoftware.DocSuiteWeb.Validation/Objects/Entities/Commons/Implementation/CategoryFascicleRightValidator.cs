using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class CategoryFascicleRightValidator : ObjectValidator<CategoryFascicleRight, CategoryFascicleRightValidator>, ICategoryFascicleRightValidator
    {
        #region [ Constructor ]
        public CategoryFascicleRightValidator(ILogger logger, ICategoryFascicleRightValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region[ Properties ]

        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public CategoryFascicle CategoryFascicle { get; set; }
        public Role Role { get; set; }
        public Container Container { get; set; }
        #endregion
    }
}