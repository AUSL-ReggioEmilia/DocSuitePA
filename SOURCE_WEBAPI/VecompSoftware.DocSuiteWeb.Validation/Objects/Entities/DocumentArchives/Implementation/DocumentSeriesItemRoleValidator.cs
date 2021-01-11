using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives
{
    public class DocumentSeriesItemRoleValidator : ObjectValidator<DocumentSeriesItemRole, DocumentSeriesItemRoleValidator>, IDocumentSeriesItemRoleValidator
    {
        #region [ Constructor ]
        public DocumentSeriesItemRoleValidator(ILogger logger, IDocumentSeriesItemRoleValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        {

        }

        #endregion

        #region[ Properties ]
        public int EntityId { get; set; }
        public Guid UniqueId { get; set; }
        public DocumentSeriesItemRoleLinkType LinkType { get; set; }
        #endregion

        #region[ Navigation Properties ]
        public DocumentSeriesItem DocumentSeriesItem { get; set; }
        public Role Role { get; set; }
        #endregion
    }
}