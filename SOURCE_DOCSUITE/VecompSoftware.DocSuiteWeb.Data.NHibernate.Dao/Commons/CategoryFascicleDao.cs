using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Data.Entity.Fascicles;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Commons
{
    public class CategoryFascicleDao : BaseNHibernateDao<CategoryFascicle>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public CategoryFascicleDao()
            : base()
        {
        }

        public CategoryFascicleDao(string sessionFactoryName) : base(sessionFactoryName)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        /// <summary>
        /// Recupera la lista degli IdCategory che non hanno una tipologia di Fascicolazione associata
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public IList<int> GetNotFascicoled()
        {
            Category category = null;
            IList<int> results = NHibernateSession.QueryOver<CategoryFascicle>()
                                    .Right.JoinAlias(x => x.Category, () => category)
                                    .Where(() => category.IsActive)
                                    .Where(Restrictions.IsNull(Projections.Property<CategoryFascicle>(x => x.Id)))
                                    .SelectList(list => list.SelectGroup(() => category.Id)
                                    ).List<int>();

            return results;
        }

        public IList<int> GetProcedureFascicles()
        {
            Category category = null;
            IList<int> results = NHibernateSession.QueryOver<CategoryFascicle>()
                                    .Right.JoinAlias(x => x.Category, () => category)
                                    .Where((x) => category.IsActive && x.DSWEnvironment == 0)
                                    .SelectList(list => list.SelectGroup(() => category.Id)
                                    ).List<int>();

            return results;
        }

        public IList<CategoryFascicle> GetCategorySubFascicles()
        {
            IList<CategoryFascicle> results = NHibernateSession.QueryOver<CategoryFascicle>()
                                                .Where(x => x.DSWEnvironment == 0)
                                                .Where(x => x.FascicleType == FascicleType.SubFascicle)
                                                .List();

            return results;
        }

        public ICollection<CategoryFascicle> GetByIdCategory(int idCategory)
        {
            ICollection<CategoryFascicle> result = NHibernateSession.QueryOver<CategoryFascicle>()
                                        .Where(x => x.Category.Id == idCategory)
                                        .List<CategoryFascicle>();

            return result;
        }

        public CategoryFascicle GetByIdCategory(int idCategory, int environment)
        {
            Category category = null;
            FasciclePeriod fasciclePeriod = null;
            CategoryFascicle result = NHibernateSession.QueryOver<CategoryFascicle>()
                                        .Inner.JoinAlias(x => x.Category, () => category)
                                        .Left.JoinAlias(x => x.FasciclePeriod, () => fasciclePeriod)
                                        .Where(x => x.Category.Id == idCategory)
                                        .Where(x => x.DSWEnvironment == environment)
                                        .SingleOrDefault<CategoryFascicle>();

            return result;
        }

        public IList<CategoryFascicle> GetFascicolableCategoriesByEnvironment(int environment)
        {
            IList<CategoryFascicle> results = NHibernateSession.QueryOver<CategoryFascicle>()
                                                .Where(x => x.DSWEnvironment == environment)
                                                .Where(x => x.FascicleType != FascicleType.Legacy && x.FascicleType != FascicleType.SubFascicle)
                                                .List<CategoryFascicle>();

            return results;
        }

        /// <summary>
        /// Ritorna la lista di Classificatori fascicolabili
        /// </summary>
        /// <returns>IList di Category</returns>
        public IList<Category> GetFascicolableCategories()
        {
            return NHibernateSession.QueryOver<CategoryFascicle>()
                                    .Where(x => x.FascicleType != FascicleType.SubFascicle)
                                    .Select(Projections.Distinct(Projections.Property<CategoryFascicle>(s => s.Category)))
                                    .List<Category>();
        }

        public IList<Category> GetFascicolableCategoriesByName(string findName)
        {
            Category category = null;
            IQueryOver<CategoryFascicle, CategoryFascicle> query = NHibernateSession.QueryOver<CategoryFascicle>()
                                                .JoinAlias(x => x.Category, () => category)
                                                .Where(x => x.FascicleType != FascicleType.SubFascicle);

            if (!string.IsNullOrEmpty(findName))
            {
                query = query.Where(Restrictions.On(() => category.Name).IsLike(findName, MatchMode.Anywhere));
            }

            return query.Select(Projections.Distinct(Projections.Property<CategoryFascicle>(s => s.Category)))
                                                .List<Category>();
        }

        public IList<CategoryFascicle> GetProcedureFascicles(int idCategory)
        {
            IList<CategoryFascicle> results = NHibernateSession.QueryOver<CategoryFascicle>()
                                        .Where(x => x.Category.Id == idCategory)
                                        .Where(x => x.FascicleType == FascicleType.Procedure)
                                        .List<CategoryFascicle>();

            return results;
        }

        public bool ExistFascicleProcedure(int idCategory)
        {
            IFutureValue<int> result = NHibernateSession.QueryOver<CategoryFascicle>()
                             .Where(x => x.FascicleType == FascicleType.Procedure)
                             .Where(x => x.Category.Id == idCategory)
                             .SelectList(list => list.SelectCount(s => s.Id))
                             .FutureValue<int>();
            return result.Value > 0;
        }

        public bool ExistFascicleDefinition(int idCategory)
        {
            IFutureValue<int> result = NHibernateSession.QueryOver<CategoryFascicle>()
                             .Where(x => x.FascicleType != FascicleType.SubFascicle)
                             .Where(x => x.Category.Id == idCategory)
                             .SelectList(list => list.SelectCount(s => s.Id))
                             .FutureValue<int>();
            return result.Value > 0;
        }

        public bool ExistProcedureSubCategories(int idCategory)
        {
            Category category = null;
            IFutureValue<int> result = NHibernateSession.QueryOver<CategoryFascicle>()
                             .Where(x => x.FascicleType == FascicleType.Procedure)
                             .JoinAlias(x => x.Category, () => category)
                             .Where(() => category.FullIncrementalPath.IsLike(string.Concat(idCategory, "|"), MatchMode.Anywhere))
                             .SelectList(list => list.SelectCount(s => s.Id))
                             .FutureValue<int>();
            return result.Value > 0;
        }

        public IList<int> GetFirstIdCategoryWithProcedureCategoryFascicle(int idCategory)
        {
            string categoryFullIncrementalPath = NHibernateSession.QueryOver<Category>()
                .Where(x => x.Id == idCategory)
                .Select(Projections.Property<Category>(s => s.FullIncrementalPath))
                .SingleOrDefault<string>();

            IQueryOver<CategoryFascicle> categoryFasciclesQuery = NHibernateSession.QueryOver<CategoryFascicle>()
                .Where(x => x.FascicleType == FascicleType.Procedure)
                .Where(x => x.Category.Id.IsIn(
                    categoryFullIncrementalPath.Split('|').Select(s => s.Trim()).ToArray())
                )
                .OrderBy(x => x.Category.Id).Desc
                .Select(Projections.Property<CategoryFascicle>(s => s.Category.Id))
                .Take(1);

            IList<int> result = categoryFasciclesQuery.List<int>();

            return result.Distinct().ToList();
        }
        #endregion [ Methods ]
    }
}
