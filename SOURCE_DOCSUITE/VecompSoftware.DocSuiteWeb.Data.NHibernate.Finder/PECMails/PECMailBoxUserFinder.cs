using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.Data.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.DTO.PECMails;
using VecompSoftware.DocSuiteWeb.EntityMapper;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.PECMails
{
    public class PECMailBoxUserFinder : BaseFinder<PECMailBoxUser, PECMailBoxUserDto>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        public int? IdPECMailBox { get; set; }
        #endregion

        #region Constructors
        public PECMailBoxUserFinder(string dbName, IEntityMapper<PECMailBoxUser, PECMailBoxUserDto> mapper)
            : base(dbName, mapper)
        { }
        
        public PECMailBoxUserFinder(IEntityMapper<PECMailBoxUser, PECMailBoxUserDto> mapper)
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper)
        { }
        #endregion

        #region Methods
        protected override IQueryOver<PECMailBoxUser, PECMailBoxUser> DecorateCriteria(IQueryOver<PECMailBoxUser, PECMailBoxUser> queryOver)
        {
            PECMailBox pecMailBox = null;
            SecurityUsers securityUsers = null;

            queryOver.JoinAlias(x => x.PECMailBox, () => pecMailBox)
                .Left.JoinAlias(x => x.SecurityUser, () => securityUsers);

            if (IdPECMailBox.HasValue)
                queryOver.Where(() => pecMailBox.Id == IdPECMailBox.Value);

            return queryOver;
        }
        
        protected override IQueryOver<PECMailBoxUser, PECMailBoxUser> CreateQueryOver()
        {
            return NHibernateSession.QueryOver<PECMailBoxUser>();
        }

        public override int Count()
        {
            IQueryOver<PECMailBoxUser, PECMailBoxUser> queryOver = CreateQueryOver();
            queryOver = DecorateCriteria(queryOver);
            queryOver = AttachFilterExpressions(queryOver);

            return queryOver.Select(Projections.CountDistinct<PECMailBoxUser>(x => x.Id))
                            .FutureValue<int>()
                            .Value;
        }
        #endregion
    }
}
