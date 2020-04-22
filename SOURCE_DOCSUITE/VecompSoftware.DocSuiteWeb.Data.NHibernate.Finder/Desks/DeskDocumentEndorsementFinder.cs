using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.DTO.Desks;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
{
    public class DeskDocumentEndorsementFinder : BaseDeskFinder<DeskDocumentEndorsement, DeskEndorsement>
    {
        #region [ Fields ]
        #endregion [ Fields ]

        #region [ Properties ]

        public Dictionary<Guid, decimal> VersionFilters { get; set; }
        public Guid? DeskId { get; set; }
        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskDocumentEndorsementFinder(IEntityMapper<DeskDocumentEndorsement, DeskEndorsement> mapper)
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper)
        {
        }

        public DeskDocumentEndorsementFinder(string dbName, IEntityMapper<DeskDocumentEndorsement, DeskEndorsement> mapper)
            : base(dbName, mapper)
        {
            VersionFilters = new Dictionary<Guid, decimal>();            
        }

        #endregion [ Constructor ]

        #region [ Methods ]
        protected override IQueryOver<DeskDocumentEndorsement, DeskDocumentEndorsement> DecorateCriteria(IQueryOver<DeskDocumentEndorsement, DeskDocumentEndorsement> queryOver)
        {
            DeskDocumentVersion deskDocumentVersion = null;
                DeskDocument deskDocument = null;
                Desk desk = null;

            queryOver.JoinAlias(o => o.DeskDocumentVersion, () => deskDocumentVersion)
                .JoinAlias(() => deskDocumentVersion.DeskDocument, () => deskDocument)
                .JoinAlias(() => deskDocument.Desk, () => desk);

            if (DeskId.HasValue)
            {
                queryOver.Where(() => desk.Id == DeskId.Value);
            }

            queryOver.SelectList(
                select => select.SelectGroup(() => deskDocument.Id).SelectMax(() => deskDocumentVersion.Version));

            if (!VersionFilters.Any()) 
                return queryOver;

            foreach (KeyValuePair<Guid, decimal> versionFilter in VersionFilters)
            {
                queryOver.Where(() => deskDocument.Id == versionFilter.Key)
                    .And(() => deskDocumentVersion.Version == versionFilter.Value);
            }
            return queryOver;
        }

        /// <summary>
        /// Conteggio elementi
        /// </summary>
        public override int Count()
        {
            IQueryOver<DeskDocumentEndorsement, DeskDocumentEndorsement> queryOver = CreateQueryOver();
            queryOver = DecorateCriteria(queryOver);
            queryOver = AttachFilterExpressions(queryOver);

            return queryOver.Select(Projections.CountDistinct<DeskDocumentEndorsement>(x => x.Id))
                            .FutureValue<int>()
                            .Value;
        }

        #endregion [ Methods ]
    }
}
