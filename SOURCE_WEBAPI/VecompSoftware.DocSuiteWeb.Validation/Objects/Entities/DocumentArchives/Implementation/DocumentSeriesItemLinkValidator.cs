using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives
{
    public class DocumentSeriesItemLinkValidator : ObjectValidator<DocumentSeriesItemLink, DocumentSeriesItemLinkValidator>, IDocumentSeriesItemLinkValidator
    {
        #region [ Constructor ]
        public DocumentSeriesItemLinkValidator(ILogger logger, IDocumentSeriesItemLinkValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
        }

        #endregion

        #region [ Properties ]   
        public string LinkType { get; set; }
        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public DocumentSeriesItem DocumentSeriesItem { get; set; }
        public Resolution Resolution { get; set; }
        #endregion
    }
}
