using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.DocumentArchives
{
    public class DocumentSeriesItemLinkService : BaseService<DocumentSeriesItemLink>, IDocumentSeriesItemLinkService
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public DocumentSeriesItemLinkService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDocumentSeriesRuleset documentSeriesRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, documentSeriesRuleset, mapperUnitOfWork, security)
        {

        }
        #endregion
    }
}
