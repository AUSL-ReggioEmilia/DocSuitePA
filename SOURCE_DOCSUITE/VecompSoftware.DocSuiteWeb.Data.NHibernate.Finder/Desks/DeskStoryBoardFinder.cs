using System;
using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.DTO.Desks;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
{
    public class DeskStoryBoardFinder : BaseDeskFinder<DeskStoryBoard, DeskComment>
    {
        #region [ Fields ]
        private DeskDocumentVersion deskDocumentVersion = null;
        private DeskDocument deskDocument = null;
        private Desk desk = null;
        #endregion [ Fields ]

        #region [ Properties ]
        public string Author { get; set; }
        public Guid? DeskDocumentId { get; set; }
        public Guid? DeskId { get; set; }
        /// <summary>
        /// Testo del commento da ricercare
        /// </summary>
        public string Description { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DeskStoryBoardType? CommentType { get; set; }
        public bool FindDocumentComments { get; set; }
        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskStoryBoardFinder(IEntityMapper<DeskStoryBoard, DeskComment> mapper)
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper)
        {
        }

        public DeskStoryBoardFinder(string dbName, IEntityMapper<DeskStoryBoard, DeskComment> mapper)
            : base(dbName, mapper)
        {
        }

        #endregion [ Constructor ]

        #region [ Methods ]
        protected override IQueryOver<DeskStoryBoard, DeskStoryBoard> DecorateCriteria(IQueryOver<DeskStoryBoard, DeskStoryBoard> queryOver)
        {
            queryOver.JoinAlias(x => x.Desk, () => desk)
                .Left.JoinAlias(o => o.DeskDocumentVersion, () => deskDocumentVersion)
                .Left.JoinAlias(() => deskDocumentVersion.DeskDocument, () => deskDocument);

            if (DeskId.HasValue)
            {
                SetDeskIdFilter(queryOver);
            }

            if (DeskDocumentId.HasValue && FindDocumentComments)
            {
                SetDocumentIdFilter(queryOver);
            }
            
            //Filtro per autore commento
            if (!string.IsNullOrEmpty(this.Author))
            {
                queryOver.WhereRestrictionOn(x => x.Author).IsLike(this.Author, MatchMode.Anywhere);
            }

            //Filtro per descrizione commento
            if (!string.IsNullOrEmpty(this.Description))
            {
                queryOver.WhereRestrictionOn(x => x.Comment).IsLike(this.Description, MatchMode.Anywhere);
            }

            //Filtro per data
            if (this.DateFrom.HasValue && !this.DateFrom.Equals(DateTime.MinValue))
            {
                queryOver.Where(x => x.DateBoard >= this.DateFrom.Value);
            }

            if (this.DateTo.HasValue && !this.DateTo.Equals(DateTime.MinValue))
            {
                queryOver.Where(x => x.DateBoard <= this.DateTo.Value);
            }

            if (this.CommentType.HasValue)
            {
                queryOver.Where(x => x.BoardType == this.CommentType.Value);
            }
            return queryOver;
        }

        private IQueryOver<DeskStoryBoard, DeskStoryBoard> SetDocumentIdFilter(IQueryOver<DeskStoryBoard, DeskStoryBoard> queryOver)
        {
            return queryOver.Where(() => deskDocument.Id == this.DeskDocumentId && deskDocument.IsActive == 0);
        }

        private IQueryOver<DeskStoryBoard, DeskStoryBoard> SetDeskIdFilter(IQueryOver<DeskStoryBoard, DeskStoryBoard> queryOver)
        {
            queryOver.Where(() => desk.Id == this.DeskId);

            if (!FindDocumentComments)
            {
                queryOver.Where(x => x.DeskDocumentVersion == null);
            }

            return queryOver;
        }

        /// <summary>
        /// Conteggio elementi
        /// </summary>
        public override int Count()
        {
            IQueryOver<DeskStoryBoard, DeskStoryBoard> queryOver = CreateQueryOver();
            queryOver = DecorateCriteria(queryOver);
            queryOver = AttachFilterExpressions(queryOver);

            return queryOver.Select(Projections.CountDistinct<DeskStoryBoard>(x => x.Id))
                            .FutureValue<int>()
                            .Value;
        }
        #endregion [ Methods ]
    }
}
