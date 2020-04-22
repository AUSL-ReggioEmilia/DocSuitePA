using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class ContainerPropertyValidator : ObjectValidator<ContainerProperty, ContainerPropertyValidator>, IContainerPropertyValidator
    {
        #region [ Constructor ]
        public ContainerPropertyValidator(ILogger logger, IContainerPropertyValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region[ Properties ]
        public Guid UniqueId { get; set; }
        public string Name { get; set; }
        public ContainerPropertyType ContainerType { get; set; }
        public int? ValueInt { get; set; }
        public DateTime? ValueDate { get; set; }
        public double? ValueDouble { get; set; }
        public bool? ValueBoolean { get; set; }
        public Guid? ValueGuid { get; set; }
        public string ValueString { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public Container Container { get; set; }
        #endregion
    }
}
