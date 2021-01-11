using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PosteWeb
{
    public class POLRequestValidator : ObjectValidator<PosteOnLineRequest, POLRequestValidator>, IPOLRequestValidator
    {
        #region [ Constructor ]

        public POLRequestValidator(ILogger logger, IValidatorMapper<PosteOnLineRequest, POLRequestValidator> mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity) : base(logger, mapper, unitOfWork, currentSecurity)
        {
        }

        #endregion

        #region [ Properties ]

        public string RequestId { get; set; }

        public string GuidPoste { get; set; }

        public string IdOrdine { get; set; }

        public POLRequestStatusEnum Status { get; set; }

        public string StatusDescription { get; set; }

        public string ErrorMessage { get; set; }

        public double TotalCost { get; set; }

        public string ExtendedProperties { get; set; }


        #region [ SOLRequest / LOLRequest / ROLRequest Properties ]
        public string Testo { get; set; }
        #endregion
        #region [ TOLRequest Properties ]
        public string DocumentName { get; set; }
        public string DocumentMD5 { get; set; }
        public string DocumentPosteMD5 { get; set; }
        public string DocumentPosteFileType { get; set; }

        public Guid? IdArchiveChain { get; set; }
        public Guid? IdArchiveChainPoste { get; set; }
        #endregion

        #endregion


        #region [ Navigation Properties ]
        public DocumentUnit DocumentUnit { get; set; }
        #endregion
    }
}
