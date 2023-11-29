using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.MassimariScarto;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.MassimariScarto
{
    public class MassimarioScartoValidator : ObjectValidator<MassimarioScarto, MassimarioScartoValidator>, IMassimarioScartoValidator
    {
        #region [ Constructor ]
        public MassimarioScartoValidator(ILogger logger, IMassimarioScartoValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public MassimarioScartoStatus Status { get; set; }

        public string Name { get; set; }

        public short? Code { get; set; }

        public string FullCode { get; set; }

        public string Note { get; set; }

        public short? ConservationPeriod { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public Guid? FakeInsertId { get; set; }

        public string MassimarioScartoParentPath { get; set; }

        public string MassimarioScartoPath { get; set; }

        public short MassimarioScartoLevel { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }


        #endregion

        #region [ Navigation Properties ]


        #endregion
    }
}
