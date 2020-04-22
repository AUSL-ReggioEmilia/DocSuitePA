using System;
using System.Collections.Generic;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Resolutions
{
    public class ResolutionKindDocumentSeriesDao : BaseNHibernateDao<ResolutionKindDocumentSeries>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]

        public ResolutionKindDocumentSeriesDao()
            : base()
        {
        }

        public ResolutionKindDocumentSeriesDao(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        public ResolutionKindDocumentSeries GetByResolutionAndSeries(Guid idResolutionKind, int idSeries)
        {
            ResolutionKind resolutionKind = null;
            DocumentSeries documentSeries = null;

            return NHibernateSession.QueryOver<ResolutionKindDocumentSeries>()
                .JoinAlias(x => x.DocumentSeries, () => documentSeries)
                .JoinAlias(x => x.ResolutionKind, () => resolutionKind)
                .Where(() => resolutionKind.Id == idResolutionKind)
                .And(() => documentSeries.Id == idSeries)
                .SingleOrDefault<ResolutionKindDocumentSeries>();
        }

        public ICollection<ResolutionKindDocumentSeries> GetByIdResolutionKind(Guid idResolutionKind)
        {
            ResolutionKind resolutionKind = null;
            DocumentSeries documentSeries = null;

            return NHibernateSession.QueryOver<ResolutionKindDocumentSeries>()
                .JoinAlias(x => x.DocumentSeries, () => documentSeries)
                .JoinAlias(x => x.ResolutionKind, () => resolutionKind)
                .Where(() => resolutionKind.Id == idResolutionKind)
                .List<ResolutionKindDocumentSeries>();
        }

        public ICollection<ResolutionKindDocumentSeries> GetResolutionAndSeries()
        {
            ResolutionKind resolutionKind = null;
            DocumentSeries documentSeries = null;
           
            return NHibernateSession.QueryOver<ResolutionKindDocumentSeries>()
                .JoinAlias(x => x.DocumentSeries, () => documentSeries)
                .JoinAlias(x => x.ResolutionKind, () => resolutionKind)
                .List<ResolutionKindDocumentSeries>();
        }

   
      
        #endregion [ Methods ]
    }
}
