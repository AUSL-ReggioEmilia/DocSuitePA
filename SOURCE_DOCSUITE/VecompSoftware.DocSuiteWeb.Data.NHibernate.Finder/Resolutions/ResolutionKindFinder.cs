using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.DTO.Resolutions;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Resolutions
{
    public class ResolutionKindFinder: BaseFinder<ResolutionKind, ResolutionKindTypeResults>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public ResolutionKindFinder(IEntityMapper<ResolutionKind, ResolutionKindTypeResults> mapper, string currentUserName) 
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ReslDB), mapper, currentUserName)
        {
        }

        public ResolutionKindFinder(string dbName, IEntityMapper<ResolutionKind, ResolutionKindTypeResults> mapper, string currentUserName)
            : base(dbName, mapper)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]
        protected override IQueryOver<ResolutionKind, ResolutionKind> DecorateCriteria(IQueryOver<ResolutionKind, ResolutionKind> queryOver)
        {
            ResolutionKindDocumentSeries resolutionKindDocumentSeries = null;
            DocumentSeries documentSeries = null;
            return queryOver
                .Left.JoinAlias(x => x.ResolutionKindDocumentSeries, () => resolutionKindDocumentSeries)
                .Left.JoinAlias(() => resolutionKindDocumentSeries.DocumentSeries, () => documentSeries)
                .Where(x => x.IsActive == true);
        }

        /// <summary>
        /// Conteggio degli elementi restituiti dalla query attualmente utilizzata
        /// </summary>
        /// <returns></returns>
        public override int Count()
        {
            
            IQueryOver<ResolutionKind, ResolutionKind> queryOver = CreateQueryOver();
            queryOver = DecorateCriteria(queryOver);
            queryOver = AttachFilterExpressions(queryOver);

            return queryOver.Select(Projections.CountDistinct<ResolutionKind>(resl => resl.Id))
                            .FutureValue<int>()
                            .Value;
        }

        #endregion [ Methods ]
    }
}
