using System;
using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.Data.Entity.UDS;
using VecompSoftware.DocSuiteWeb.EntityMapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.UDS
{
    public class UDSRepositoryFinder : BaseFinder<UDSRepository, UDSRepositoryModel>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public UDSRepositoryFinder(IEntityMapper<UDSRepository, UDSRepositoryModel> mapper, String currentUserName)
            : this(System.Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper, currentUserName)
        {
        }

        public UDSRepositoryFinder(string dbName, IEntityMapper<UDSRepository, UDSRepositoryModel> mapper, String currentUserName)
            : base(dbName, mapper)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        protected override IQueryOver<UDSRepository, UDSRepository> DecorateCriteria(IQueryOver<UDSRepository, UDSRepository> queryOver)
        {
            IQueryOver<UDSRepository, UDSRepository> query = queryOver
                .WithSubquery
                .WhereExists(QueryOver.Of<UDSRepository>()
                .Select(
                    Projections.Distinct(
                        Projections.ProjectionList()
                        .Add(Projections.GroupProperty("Name"))
                        .Add(Projections.Max("Version")))));

            query = query.Where(x => x.ExpiredDate == null);

            return queryOver;
        }

        public override int Count()
        {

            IQueryOver<UDSRepository, UDSRepository> queryOver = CreateQueryOver();
            queryOver = DecorateCriteria(queryOver);
            queryOver = AttachFilterExpressions(queryOver);

            return queryOver.Select(Projections.CountDistinct<UDSRepository>(u => u.Id))
                            .FutureValue<int>()
                            .Value;
        }
        #endregion
    }
}
