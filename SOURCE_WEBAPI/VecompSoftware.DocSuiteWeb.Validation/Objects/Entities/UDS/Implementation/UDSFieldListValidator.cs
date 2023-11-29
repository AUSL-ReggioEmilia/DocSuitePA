using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS
{
    public class UDSFieldListValidator : ObjectValidator<UDSFieldList, UDSFieldListValidator>, IUDSFieldListValidator
    {

        #region [ Constructor ]
        public UDSFieldListValidator(ILogger logger, IUDSFieldListValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity security, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, security, parameterEnvSecurity) { }
        #endregion

        #region [ Properties ]
        public string FieldName { get; set; }
        public string Name { get; set; }
        public UDSFieldListStatus Status { get; set; }
        public int Environment { get; set; }
        public string UDSFieldListPath { get; set; }
        public short UDSFieldListLevel { get; set; }
        public Guid? ParentInsertId { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public UDSRepository Repository { get; set; }
        #endregion
    }
}
