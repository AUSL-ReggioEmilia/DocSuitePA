using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.OCharts;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.OCharts
{
    public class OChartItemValidator : ObjectValidator<OChartItem, OChartItemValidator>, IOChartItemValidator
    {
        #region [ Constructor ]
        public OChartItemValidator(ILogger logger, IOChartItemValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public bool? Enabled { get; set; }
        public string Code { get; set; }
        public string FullCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public bool? Imported { get; set; }
        public string Acronym { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public OChart OChart { get; set; }
        public OChartItem Parent { get; set; }
        public ICollection<OChartItem> Children { get; set; }
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<OChartItemContainer> OChartItemContainers { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<PECMailBox> Mailboxes { get; set; }
        #endregion
    }
}
