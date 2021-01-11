using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Resolutions
{
    public class ResolutionKindFacade : BaseResolutionFacade<ResolutionKind, Guid, ResolutionKindDao>
    {
        #region [ Fields ]
        private readonly string _userName;
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]

        public ResolutionKindFacade(string userName)
            : base()
        {
            _userName = userName;
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        public ICollection<ResolutionKind> GetActiveResolutionKind()
        {
            return _dao.GetActiveResolutionKind();
        }

        public IList<ResolutionKind> GetNotActiveResolutionKind()
        {
            return _dao.GetNotActiveResolutionKind();
        }

        public IList<ResolutionKind> GetByName(string name)
        {
            return _dao.GetByName(name);
        }

        public Guid CheckResolutionKindDocumentSeries(Resolution resl, DocumentSeriesItem documentSeriesItem)
        {
            ResolutionKind rk = _dao.GetById(resl.ResolutionKind.Id, false);

            if(rk.ResolutionKindDocumentSeries.Any() && rk.ResolutionKindDocumentSeries.Where(x => x.DocumentSeries.Id == documentSeriesItem.DocumentSeries.Id).Any())
            {
                return rk.ResolutionKindDocumentSeries.Where(x => x.DocumentSeries.Id == documentSeriesItem.DocumentSeries.Id).First().Id;
            }
            return Guid.Empty;
        }
        #endregion [ Methods ]
    }
}
