using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.DTO.Desks;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
{
    public class DeskApprovalEndorsementFinder : BaseDeskFinder<Desk, DeskEndorsement>
    {
        #region [ Fields ]
        #endregion [ Fields ]

        #region [ Properties ]

        public Dictionary<Guid, decimal> VersionFilters { get; set; }
        public Guid? DeskId { get; set; }
        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskApprovalEndorsementFinder(IEntityMapper<Desk, DeskEndorsement> mapper)
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper)
        {
        }

        public DeskApprovalEndorsementFinder(string dbName, IEntityMapper<Desk, DeskEndorsement> mapper)
            : base(dbName, mapper)
        {
            VersionFilters = new Dictionary<Guid, decimal>();            
        }

        #endregion [ Constructor ]

        #region [ Methods ]
        protected override IQueryOver<Desk, Desk> DecorateCriteria(IQueryOver<Desk, Desk> queryOver)
        {
            DeskDocumentEndorsement deskDocumentEndorsement = null;
            DeskRoleUser deskRoleUser = null;
            DeskDocumentVersion deskDocumentVersion = null;
            DeskDocument deskDocument = null;

            queryOver.JoinAlias(o => o.DeskDocuments, () => deskDocument)
                     .JoinAlias(() => deskDocument.DeskDocumentVersions, () => deskDocumentVersion)
                     .JoinAlias(o => o.DeskRoleUsers, () => deskRoleUser)
                     .Left.JoinAlias(() => deskRoleUser.DeskDocumentEndorsements, () => deskDocumentEndorsement, () => deskDocumentEndorsement.DeskDocumentVersion.Id == deskDocumentVersion.Id);
                     

            if (DeskId.HasValue)
            {
                queryOver.Where(desk => desk.Id == DeskId.Value);
            }

            FilterByUserPermission(queryOver);

            queryOver.SelectList(select => select.SelectGroup(() => deskDocument.Id).SelectMax(() => deskDocumentVersion.Version));
            
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
        /// Filtro gli utenti visibili nella griglia di approvazione.
        /// Solo coloro che possono approvare i documenti dovranno essere previsti nel riepilogo di approvazione.
        /// </summary>
        /// <param name="queryOver"></param>
        /// <returns></returns>
        public IQueryOver<Desk, Desk> FilterByUserPermission(IQueryOver<Desk, Desk> queryOver)
        {
            DeskRoleUser deskRoleUser = null;

            // Filtro su utenti che hanno i diritti
            ICollection<DeskPermissionType> userWithRole = new Collection<DeskPermissionType>();
            userWithRole.Add(DeskPermissionType.Admin);
            userWithRole.Add(DeskPermissionType.Approval);
            userWithRole.Add(DeskPermissionType.Manage);

            queryOver.Where(() => deskRoleUser.PermissionType.IsIn(userWithRole.ToArray()));
            return queryOver;
        }

        /// <summary>
        /// Conteggio elementi
        /// </summary>
        public override int Count()
        {
            IQueryOver<Desk, Desk> queryOver = CreateQueryOver();
            queryOver = DecorateCriteria(queryOver);
            queryOver = AttachFilterExpressions(queryOver);

            return queryOver.Select(Projections.CountDistinct<DeskDocumentEndorsement>(x => x.Id))
                            .FutureValue<int>()
                            .Value;
        }

        #endregion [ Methods ]
    }
}
